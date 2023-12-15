using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_6.KarasEF
{
    public interface IKarasContext
    {
        TResult QuaryAsync<TResult>(FormattableString sql);
        NpgsqlConnection GetConnection();
    }
}
