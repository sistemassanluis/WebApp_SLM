using Dapper;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using WebApp_SLM.Models;

namespace WebApp_SLM.Services
{
    public interface ITablasRepository
    {
        Task<IEnumerable<TablasItem>> TablasFindById(long id);
    }
    public class TablasRepository: ITablasRepository
    {
        private readonly string connSqlRRHH;

        public TablasRepository(IConfiguration configuration)
        {
            connSqlRRHH = configuration.GetConnectionString("conecSQlServerRRHH");
        }

        public async Task<IEnumerable<TablasItem>> TablasFindById(long id)        {
            using var conn = new SqlConnection(connSqlRRHH);
            conn.Open();
            return await conn.QueryAsync<TablasItem>(@"select t.id as idTabla,
                                                            nst.descripcion as descripcionTabla,
                                                            nst.id as idSubtabla,
                                                            nst.descripcion,
                                                            nst.descripcion2,
                                                            nst.abreviado,
                                                            nst.codigo
                                                        from gn_tablas t inner join gn_subtablas nst on (t.id = nst.tabla_id)
                                                        where t.id = @id
                                                        order by nst.descripcion", new {id});
            
        }
    }
}
