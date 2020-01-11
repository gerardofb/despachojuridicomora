using System;
using DTOWDXDespachoMora;
using DALWDXDespachoMora;
namespace BALWDXDespachoMora
{
    public class ConfiguracionBAL : baseDTOComun
    {
        public ConfiguracionBAL()
        {
        }
        public string ConsultarValorConfiguracion(string Clave)
        {
            using(ConfiguracionDAL Metodo = new ConfiguracionDAL())
            {
                return Metodo.ConsultarValorConfiguracion(Clave);
            }
        }

    }
}
