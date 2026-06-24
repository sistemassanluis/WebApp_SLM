using System.Data.SqlTypes;
using WebApp_SLM.Models.HorasExtras;
using Microsoft.Data.SqlClient;
using Dapper;
using WebApp_SLM.Models;
using System.Data;
using System.Collections.Immutable;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WebApp_SLM.Services
{
    public interface ITiempoExtraRepository
    {
        OverTimeDet SeachOverTime(long id);
        Task Crud_HoraExtra(OverTime overtime, List<MiListaType> motivos, string tipoTrs);
        Task <IEnumerable<ListaTiempoExtra>> ListarHorasExtras(DateTime fechaini, DateTime fechafin, long personal);
        List<FindPersonal> SeachPersonal(string texto);
        Task<IEnumerable<HorarioPersonal>> ViewHorarioPersonal(long id_personal);
        Task<IEnumerable<ListarTiempoExtraConsolidado>> ListarHorasExtrasConsolidado(DateTime fechaini, DateTime fechafin);
    }
    public class TiempoExtraRepository : ITiempoExtraRepository
    {
        private readonly string connSqlRRHH;
   

        public TiempoExtraRepository(IConfiguration configuration)
        {
            connSqlRRHH = configuration.GetConnectionString("conecSQlServerRRHH");
        }

        public OverTimeDet  SeachOverTime(long id)
        {
            OverTimeDet overTimes = new OverTimeDet(
                    id: -1,
                    personal_id: 1,
                    nombre_completo: "",
                    area:"",
                    puesto:"",
                    horario_dia: "",
                    dia_hora_inicio: new DateTime(),
                    dia_hora_fin: new DateTime(),
                    horas_extra: 0,
                    minutos_extra: 0,
                    observacion : "",
                    date_add : new DateTime(),
                    id_user_add : 1,
                    date_modify : new DateTime(),
                    id_user_modify : 1
                    );

            using (var conn = new SqlConnection(connSqlRRHH))
            {
                OverTimeDet data = conn.QueryFirst<OverTimeDet>(@"SELECT oe.id
                                                    ,oe.personal_id
                                                    ,concat (psn.ape_paterno, ' ' , psn.ape_materno,  ' ', psn.nombres) as nombre_completo
                                                    ,sta.descripcion as area
	                                                ,stp.descripcion as puesto
                                                    ,oe.dia_hora_inicio
                                                    ,oe.dia_hora_fin
                                                    ,dbo.f_calculatiempoTrancurrido( oe.dia_hora_inicio, oe.dia_hora_fin, 'H') as horas_extra
                                                    ,dbo.f_calculatiempoTrancurrido( oe.dia_hora_inicio, oe.dia_hora_fin, 'M') as minutos_extra
                                                    ,observacion
                                                    ,oe.date_add
                                                    ,oe.id_user_add
                                                    ,oe.date_modify
                                                    ,oe.id_user_modify
                                                    ,concat(SUBSTRING(CONVERT(VARCHAR,hp.hora_ingreso, 108), 1, 5), ' / ', SUBSTRING(CONVERT(VARCHAR,hp.hora_salida, 108), 1, 5)) as horario_dia 
                                                FROM ast_overtime_event oe inner join gn_personal p on (oe.personal_id = p.id)inner join gn_persona psn on (p.persona_id = psn.id) 
                                                                             left join gn_subtablas sta on (p.area = sta.id and sta.tabla_id = 2)
			                                                                 left join gn_subtablas stp on (p.puesto = stp.id and stp.tabla_id = 3)
                                                                             left join ast_horario_personal hp on (p.id = hp.personal_id and hp.dia = DATEPART(dw, oe.dia_hora_inicio)) 
                                                WHERE oe.id = @id 
                                                order by oe.dia_hora_inicio", new { id });

                overTimes.id = data.id;
                overTimes.personal_id = data.personal_id;                
                overTimes.nombre_completo = data.nombre_completo;
                overTimes.area = data.area;
                overTimes.puesto = data.puesto;
                overTimes.dia_hora_inicio = data.dia_hora_inicio;
                overTimes.dia_hora_fin = data.dia_hora_fin;
                overTimes.horario_dia = data.horario_dia;
                overTimes.minutos_extra = data.minutos_extra;
                overTimes.observacion = data.observacion;
                overTimes.date_add = data.date_add;
                overTimes.id_user_add = data.id_user_add;
                overTimes.date_modify = data.date_modify;
                overTimes.id_user_modify = data.id_user_modify;


                IEnumerable<OverTimeReasonDet> dataMotivos = conn.Query<OverTimeReasonDet>(@"select oer.id,
	                                                                            oer.overtime_id, 
	                                                                            oer.motivo_id,
	                                                                            st.descripcion as motivo_descripcion, 
	                                                                            oer.date_add,
	                                                                            oer.id_user_add,
	                                                                            oer.date_modify,
	                                                                            oer.id_user_modify
                                                                            from ast_overtime_event_reason oer inner join gn_subtablas st on (oer.motivo_id = st.id)
                                                                            where oer.overtime_id = @id", new { id });
                foreach (var item in dataMotivos)
                {
                    overTimes.motivos.Add(item);
                }

                return overTimes;
                
            } ;

            return overTimes;
        }

        public List<FindPersonal> SeachPersonal(string texto)
        {
            List<FindPersonal> lista = new List<FindPersonal>();
            string filtro =  texto != null ? texto : "";
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
        public async Task<IEnumerable<ListaTiempoExtra>> ListarHorasExtras(DateTime fechaini, DateTime fechafin, long personal)
        {
            using var conn = new SqlConnection(connSqlRRHH);
            IEnumerable<ListaTiempoExtra> lista = await conn.QueryAsync<ListaTiempoExtra>("sp_ast_lista_overtime",
                new
                {
                    fechaini,
                    fechafin,
                    personal
                },
                commandType: System.Data.CommandType.StoredProcedure
                );
            return lista;
        }

        public async Task<IEnumerable<ListarTiempoExtraConsolidado>> ListarHorasExtrasConsolidado(DateTime fechaini, DateTime fechafin)
        {
            using var conn = new SqlConnection(connSqlRRHH);
            IEnumerable<ListarTiempoExtraConsolidado> lista = await conn.QueryAsync<ListarTiempoExtraConsolidado>("sp_ast_lista_overtime_consolidated",
                new
                {
                    fechaini,
                    fechafin                    
                },
                commandType: System.Data.CommandType.StoredProcedure
                );
            return lista;
        }

        public async Task<Boolean> ValidarTiempoExtra(DateTime fechaini, DateTime fechafin, long personal)
        {
            Boolean valid = false;

            using var conn = new SqlConnection(connSqlRRHH);
            int lista = await conn.QueryFirstAsync<int>("sp_ast_valida_hora_extra",
                new
                {
                    fechaini,
                    fechafin,
                    personal
                },
                commandType: System.Data.CommandType.StoredProcedure
                );

            return valid;
        }
        public async Task Crud_HoraExtra(OverTime overtime, List<MiListaType> motivos, String tipoTrs)
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

        public async Task<IEnumerable<HorarioPersonal>> ViewHorarioPersonal(long id_personal)
        {
            using var conn = new SqlConnection(connSqlRRHH);
            return await conn.QueryAsync<HorarioPersonal>(@"select id
                                                        ,personal_id
                                                        ,dia
                                                        ,(case hp.dia when 1 then 'Lu' when 2 then 'Ma' when 3 then 'Mi'  when 4 then 'Ju' when 5 then 'Vi' when 6 then 'Sa' when 7 then 'Do' else '??' end) as dia_Label
                                                        ,turno_cruzado
                                                        ,hora_ingreso
                                                        ,hora_salida
                                                        ,hora_refr_inicio
                                                        ,hora_refr_fin
                                                        ,date_add
                                                        ,id_user_add
                                                        ,date_modify
                                                        ,id_user_modify
                                                        from ast_horario_personal hp
                                                        where personal_id = @id_personal
                                                        order by dia", new { id_personal });

        }
    }
}
