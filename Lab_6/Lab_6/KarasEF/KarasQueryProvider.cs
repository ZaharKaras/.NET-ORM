using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6.KarasEF
{
    public class KarasQueryProvider : IQueryProvider
    {
        private  IKarasContext _context;

        public KarasQueryProvider(IKarasContext context)
        {
            _context = context;
        }
        public IQueryable CreateQuery(Expression expression) => CreateQuery<object>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new KarasQueryable<TElement>(expression, this);
        }

        public object? Execute(Expression expression) => Execute<object>(expression);

        public TResult Execute<TResult>(Expression expression)
        {
            var result = new QueryBuilder(_context);
            var sql = result.Compile(expression);

            return _context.QuaryAsync<TResult>(sql); //QueryMapper.QueryAsync<TResult>(null, null, default).Result.Single();
        }

        public NpgsqlConnection GetConnection()
        {
            return _context.GetConnection();
        }
    }
}
