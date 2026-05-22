using System.Data.SqlTypes;
using WebApp_SLM.Models.HorasExtras;
using Microsoft.Data.SqlClient;
using Dapper;
using WebApp_SLM.Models;
using System.Data;

namespace WebApp_SLM.Services
{
    public interface ITiempoExtraRepository
    {
        Task Crud_HoraExtra(OverTime overtime, List<MiListaType> motivos, string tipoTrs);
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

        public async Task Crud_HoraExtra (OverTime overtime, List<MiListaType> motivos, String tipoTrs)
        {
            DataTable dtMot = new DataTable();
            dtMot.Columns.Add("Valorid", typeof(long));
            dtMot.Columns.Add("valor2", typeof(string));
            dtMot.Columns.Add("valor3", typeof(string));

            foreach (var item in motivos)
            {
                dtMot.Rows.Add(item.Valorid, item.valor2, item.valor3);
            }

            using var conn = new SqlConnection(connSqlRRHH);

            var p = new DynamicParameters();
            p.Add("@id", overtime.id, DbType.Int64);
            p.Add("@personal_id", overtime.personal_id, DbType.Int64);
            p.Add("@dia_hora_inicio", overtime.dia_hora_inicio, DbType.DateTime);
            p.Add("@dia_hora_fin", overtime.dia_hora_fin, DbType.DateTime);
            p.Add("@observacion", overtime.observacion, DbType.String);
            p.Add("@id_user_add", overtime.id_user_add, DbType.Int64);
            p.Add("@id_user_modify", overtime.id_user_modify, DbType.Int64);
            p.Add("@tipoOperacion", tipoTrs, DbType.String);
            p.Add("@motivo", dtMot.AsTableValuedParameter("dbo.MiListaType"));

            var id = await conn.QuerySingleAsync<long>("sp_ast_guardar_overtime_event",
               p,
                commandType: System.Data.CommandType.StoredProcedure
                );
            overtime.id = id;
        }
    }
}
