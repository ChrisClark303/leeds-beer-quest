
using LeedsBeerQuest.App.Models;

namespace LeedsBeerQuest.App
{
    public class BeerEstablishmentDataParser : IBeerEstablishmentDataParser
    {
        //TODO: Not sure about leaving this as a field
        private string[] _columns;

        public BeerEstablishment[] Parse(string data)
        {
            string[] allRows = data.TrimEnd('\n').TrimEnd('\r').Split("\r\n");
            return allRows switch
            {
                [] => Array.Empty<BeerEstablishment>(),
                [var columns, .. var dataRows] => ParseDataRows(columns, dataRows)
            };
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
                    Lat = GetDoubleValueForField("lat", rowData),
                    Long = GetDoubleValueForField("lng", rowData)
                },
                Address = GetValueForField("address", rowData),
                Phone = GetValueForField("phone", rowData),
                Twitter = GetValueForField("twitter", rowData),
                Ratings = new EstablishmentRatings
                {
                    Beer = GetDoubleValueForField("stars_beer", rowData),
                    Atmosphere = GetDoubleValueForField("stars_atmosphere", rowData),
                    Amenities = GetDoubleValueForField("stars_amenities", rowData),
                    Value = GetDoubleValueForField("stars_value", rowData)
                },
                Tags = GetStringArrayForField("tags", rowData)
            };
        }

        private string GetValueForField(string fieldName, string[] rowData)
        {
            var index = GetPositionForField(fieldName);
            if (index == -1)
            {
                return string.Empty;
            }

            return rowData[index].Trim('\"', ' ');
        }

        private double GetDoubleValueForField(string fieldName, string[] rowData)
        {
            return double.Parse(GetValueForField(fieldName, rowData));
        }

        private int GetPositionForField(string fieldName)
        {
            var index = Array.IndexOf(_columns, fieldName);
            return index;
        }

        private string[] GetStringArrayForField(string fieldName, string[] rowData)
        {
            var value = GetValueForField(fieldName, rowData);
            if (string.IsNullOrEmpty(value))
            {
                return Array.Empty<string>();
            }
            return value.Split(',');
        }
    }
}
