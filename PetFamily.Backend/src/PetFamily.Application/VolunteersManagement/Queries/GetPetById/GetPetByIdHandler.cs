using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Mappers;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.VolunteersManagement.Queries.GetPetById;

public class GetPetByIdHandler : IQueryHandler<PetDto, GetPetByIdQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetPetByIdHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<PetDto, ErrorList>> HandleAsync(
        GetPetByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var connection = _sqlConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("PetId", query.PetId);

        const string sql = """
                           SELECT
                           id,
                           volunteer_id AS VolunteerId,
                           nickname,  
                           date_of_birth AS BirthDate,
                           description,
                           position,
                           color,
                           species_id AS SpeciesId,
                           breed_id AS BreedId,
                           height,
                           weight,
                           help_status AS HelpStatus,
                           locality,
                           region,
                           postal_code AS PostalCode,
                           country_code AS CountryCode,
                           address_lines AS AddressLines,
                               (
                                   SELECT jsonb_agg(photo ORDER BY
                                           CASE
                                               WHEN main_photo_path IS NOT NULL AND photo."FilePath" = main_photo_path 
                                               THEN 0
                                               ELSE 1
                                           END)
                                   FROM jsonb_to_recordset(photos) AS photo("FilePath" TEXT))
                                   AS photos
                           FROM pets
                           WHERE id = @PetId AND is_deleted = false
                           """;

        var pets = await connection.QueryAsync(
            sql,
            PetDtoMapper.MapFlatDtoToPetDtoWithPhotos(),
            parameters,
            splitOn: "photos");

        var petDto = pets.SingleOrDefault();

        if (petDto is null)
            return Errors.General.NotFound(query.PetId).ToErrorList();

        return petDto;
    }
}