using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Npgsql;

namespace Lab_6.KarasEF
{
    public class KarasContext : IKarasContext
    {
        private NpgsqlConnection _connection;
        public KarasContext(NpgsqlConnection connection)
        {
            _connection = connection;

            var sets = GetType().GetProperties()
                .Where(x => x.PropertyType.GetGenericTypeDefinition() == typeof(KarasSet<>));

            foreach (var set in sets)
            {
                set.SetValue(this, CreatSet(set.PropertyType.GetGenericArguments()[0]));
            }
                
        }

        public NpgsqlConnection GetConnection()
        {
            return _connection;
        }

        private object? CreatSet(Type setPropertyType)
        {
            return typeof(KarasContext).GetMethod("CreateSetInternal", BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(setPropertyType)
                .Invoke(this, Array.Empty<object>());
        }

        private object CreateSetInternal<T>() => new KarasSet<T>(new KarasQueryProvider(this));

        public TResult QuaryAsync<TResult>(FormattableString sql)
        {
            if(typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var entityType = typeof(TResult).GetGenericArguments()[0];
                var task = (Task)QueryMapper.QueryAsyncType(_connection, sql, entityType, default);
                task.Wait();
                dynamic d = task;
                return d.Result;
            }

            throw new InvalidOperationException();
        }
    }

}
