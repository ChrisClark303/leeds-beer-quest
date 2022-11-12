using LeedsBeerQuest.Api.Models;

namespace LeedsBeerQuest.Api
{
    public class BeerEstablishmentDataParser : IBeerEstablishmentDataParser
    {
        //TODO: Not sure about leaving this as a field
        private string[] _columns;
        public BeerEstablishment[] Parse(string data)
        {
            string[] allRows = data.Split("\r\n");
            return allRows switch
            {
                [] => Array.Empty<BeerEstablishment>(),
                [var columns, .. var dataRows] => ParseDataRows(columns, dataRows)
            };
            //return ParseDataRows(allRows.First(), allRows.Skip(1).ToArray());
        }

        private BeerEstablishment[] ParseDataRows(string columnHeaders, params string[] rows)
        {
            _columns = columnHeaders.Replace("\"", "").Split(',');

            return rows.Select(row =>
            {
                var rowData = row.Split("\",\"");
                return ParseRow(rowData);
            })
            .ToArray();
        }

        private BeerEstablishment ParseRow(string[] rowData)
        {
            return new BeerEstablishment()
            {
                Name = GetValueForField("name", rowData),
                Category = GetValueForField("category", rowData),
                Url = new Uri(GetValueForField("url", rowData)),
                Date = DateTime.Parse(GetValueForField("date", rowData)),
                Excerpt = GetValueForField("excerpt", rowData),
                Thumbnail = new Uri(GetValueForField("thumbnail", rowData)),
                Location = new Location
                {
                    Lat = Decimal.Parse(GetValueForField("lat", rowData)),
                    Long = Decimal.Parse(GetValueForField("lng", rowData))
                },
                Address = GetValueForField("address", rowData),
                Phone = GetValueForField("phone", rowData),
                Twitter = GetValueForField("twitter", rowData),
                Ratings = new EstablishmentRatings
                {
                    Beer = Decimal.Parse(GetValueForField("stars_beer", rowData)),
                    Atmosphere = Decimal.Parse(GetValueForField("stars_atmosphere", rowData)),
                    Amenities = Decimal.Parse(GetValueForField("stars_amenities", rowData)),
                    Value = Decimal.Parse(GetValueForField("stars_value", rowData))
                },
                Tags = GetValueForField("tags", rowData).Split(',')
            };
        }

        private string GetValueForField(string fieldName, string[] rowData)
        {
            var index = GetPositionForField(fieldName);
            if (index == -1)
            {
                return string.Empty;
            }

            return rowData[index].Trim('\"');
        }

        private int GetPositionForField(string fieldName)
        {
            var index = Array.IndexOf(_columns, fieldName);
            return index;
        }
    }
}
