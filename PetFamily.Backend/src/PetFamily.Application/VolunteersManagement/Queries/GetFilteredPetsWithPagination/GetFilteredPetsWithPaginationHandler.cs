using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Dapper;
using PetFamily.Application.Abstractions;
using PetFamily.Application.Database;
using PetFamily.Application.Extensions;
using PetFamily.Application.Models;
using PetFamily.Contracts.Dtos.Pet;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.VolunteersManagement.Queries.GetFilteredPetsWithPagination;

public class GetFilteredPetsWithPaginationHandler
    : IQueryHandler<PagedList<PetDto>, GetFilteredPetsWithPaginationQuery>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetFilteredPetsWithPaginationHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<Result<PagedList<PetDto>, ErrorList>> HandleAsync(
        GetFilteredPetsWithPaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var connection = _sqlConnectionFactory.Create();
        var parameters = new DynamicParameters();

        const string baseQuery = """
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
                                 """;

        const string countQuery = "SELECT COUNT(*) FROM pets";

        List<string> whereClauses = GetWhereClauses(query, parameters);

        var sqlQuery = new StringBuilder(baseQuery);
        var countQueryBuilder = new StringBuilder(countQuery);

        if (whereClauses.Any())
        {
            string whereClause = " WHERE " + string.Join(" AND ", whereClauses);
            sqlQuery.Append(whereClause);
            countQueryBuilder.Append(whereClause);
        }

        long totalCount = await connection.ExecuteScalarAsync<long>(
            countQueryBuilder.ToString(), parameters);

        sqlQuery.ApplySorting(query.SortBy, query.SortAscending);
        sqlQuery.ApplyPagination(parameters, query.PageNumber, query.PageSize);

        var pets = await connection.QueryAsync(
            sqlQuery.ToString(),
            MapFlatDtoToPetDtoWithPhotos(),
            splitOn: "photos",
            param: parameters);

        return new PagedList<PetDto>
        {
            Items = pets.ToList(), TotalCount = totalCount, PageSize = query.PageSize, PageNumber = query.PageNumber,
        };
    }

    private List<string> GetWhereClauses(GetFilteredPetsWithPaginationQuery query, DynamicParameters parameters)
    {
        var whereClauses = new List<string>();

        if (query.VolunteerIds != null && query.VolunteerIds.Length != 0)
        {
            whereClauses.Add("volunteer_id = ANY(@VolunteerIds)");
            parameters.Add("VolunteerIds", query.VolunteerIds);
        }

        if (!string.IsNullOrWhiteSpace(query.Nickname))
        {
            whereClauses.Add("nickname ILIKE @Nickname");
            parameters.Add("Nickname", $"%{query.Nickname}%");
        }

        if (query.Age.HasValue)
        {
            whereClauses.Add("EXTRACT(YEAR FROM AGE(CURRENT_DATE, date_of_birth)) = @Age");
            parameters.Add("Age", query.Age.Value);
        }

        if (query.SpeciesId.HasValue)
        {
            whereClauses.Add("species_id = @SpeciesId");
            parameters.Add("SpeciesId", query.SpeciesId.Value);
        }

        if (query.BreedId.HasValue)
        {
            whereClauses.Add("breed_id = @BreedId");
            parameters.Add("BreedId", query.BreedId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Color))
        {
            whereClauses.Add("color ILIKE @Color");
            parameters.Add("Color", $"%{query.Color}%");
        }

        if (!string.IsNullOrWhiteSpace(query.CountryCode))
        {
            whereClauses.Add("country_code ILIKE @CountryCode");
            parameters.Add("CountryCode", $"%{query.CountryCode}%");
        }

        if (!string.IsNullOrWhiteSpace(query.Locality))
        {
            whereClauses.Add("locality ILIKE @Locality");
            parameters.Add("Locality", $"%{query.Locality}%");
        }

        if (query.Height.HasValue)
        {
            whereClauses.Add("height = @Height");
            parameters.Add("Height", query.Height.Value);
        }

        if (query.Weight.HasValue)
        {
            whereClauses.Add("weight = @Weight");
            parameters.Add("Weight", query.Weight.Value);
        }

        if (query.HelpStatus.HasValue)
        {
            whereClauses.Add("help_status = @HelpStatus");
            parameters.Add("HelpStatus", query.HelpStatus.Value);
        }

        if (query.PositionFrom.HasValue)
        {
            whereClauses.Add("position >= @PositionFrom");
            parameters.Add("PositionFrom", query.PositionFrom.Value);
        }

        if (query.PositionTo.HasValue)
        {
            whereClauses.Add("position <= @PositionTo");
            parameters.Add("PositionTo", query.PositionTo.Value);
        }

        return whereClauses;
    }

    private Func<PetDtoFlat, string, PetDto> MapFlatDtoToPetDtoWithPhotos() =>
        (petFlat, jsonFiles) => new PetDto
        {
            Id = petFlat.Id,
            VolunteerId = petFlat.VolunteerId,
            Nickname = petFlat.Nickname,
            BirthDate = DateOnly.FromDateTime(petFlat.BirthDate),
            Description = petFlat.Description,
            Position = petFlat.Position,
            Color = petFlat.Color,
            HelpStatus = petFlat.HelpStatus,
            SpeciesBreed = new SpeciesBreedDto(petFlat.SpeciesId, petFlat.BreedId),
            Measurements = new MeasurementsDto(petFlat.Height, petFlat.Weight),
            Address = new AddressDto(
                JsonSerializer.Deserialize<IEnumerable<string>>(petFlat.AddressLines) ?? [],
                petFlat.Locality,
                petFlat.Region,
                petFlat.PostalCode,
                petFlat.CountryCode),
            Photos = !string.IsNullOrWhiteSpace(jsonFiles)
                ? JsonSerializer.Deserialize<PetFileDto[]>(jsonFiles)!
                : [],
        };
}