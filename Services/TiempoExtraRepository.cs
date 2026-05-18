using System.Data.SqlTypes;
using WebApp_SLM.Models.HorasExtras;
using Microsoft.Data.SqlClient;

namespace WebApp_SLM.Services
{
    public interface ITiempoExtraRepository
    {
        List<FindPersonal> SeachPersonal(string texto);
        
    }
    public class TiempoExtraRepository : ITiempoExtraRepository
    {
        private readonly string connSqlRRHH;

        public TiempoExtraRepository(IConfiguration configuration)
        {
            connSqlRRHH = configuration.GetConnectionString("conecSQlServerRRHH");
        }
        public List<FindPersonal> SeachPersonal(string texto)
        {
            List<FindPersonal> lista = new List<FindPersonal>();

            using (var conn = new SqlConnection(connSqlRRHH))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_ast_search_personal", conn);
                cmd.Parameters.AddWithValue("texto", texto);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new FindPersonal
                        {
                            value = Convert.ToInt64(dr["id_personal"]),
                            label = dr["nombre_completo"].ToString(),
                            area = Convert.ToInt32(dr["area"]),
                            desc_area = dr["desc_area"].ToString(),
                            puesto = Convert.ToInt32(dr["puesto"]),
                            desc_puesto = dr["desc_puesto"].ToString(),
                            tipo_documento = dr["tipo_documento"].ToString(),
                            nro_documento = dr["nro_documento"].ToString(),
                            correo_electronico = dr["correo_electronico"].ToString(),
                            nro_telefonico = dr["nro_telefonico"].ToString(),
                        });
                    }
                }
                conn.Close();
            }
            return lista;
        }
    }
}
