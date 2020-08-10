using System.Text;

namespace MakelaarCounter.Models
{
    public class ListingFetcherQuery
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder("?");

        public ListingFetcherQuery(string type, string filterPath, int pageSize)
        {
            _stringBuilder.Append($"type={type}&");
            _stringBuilder.Append($"zo={filterPath}&");
            _stringBuilder.Append($"pagesize={pageSize}");
        }

        public string ToString(int page)
        {
            var stringBuilder = new StringBuilder(_stringBuilder.ToString());
            stringBuilder.Append($"&page={page}");
            return stringBuilder.ToString();
        }

        public string FirstPageQueryToString()
        {
            return _stringBuilder.ToString();
        }
    }
}
