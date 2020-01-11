using System;
using DTOWDXDespachoMora;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace DALWDXDespachoMora
{
    public class ConfiguracionDAL : baseDTOComun
    {
        public string ConsultarValorConfiguracion(string Clave)
        {
            string _result = String.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(baseDTOComun.Conexion))
                {
                    conn.Open();
                    SqlCommand Cmd = conn.CreateCommand();
                    Cmd.CommandType = CommandType.Text;
                    Cmd.CommandText = $@"SELECT Valor FROM WDXConfiguracionMora
                                        WHERE Llave = '{Clave}'";
                    IDataReader reader = Cmd.ExecuteReader();
                    reader.Read();
                    _result = reader["Valor"].ToString();
                }
            }
            catch (SqlException ex)
            {
                
            }
            return _result;
        }
    }
}
