﻿using despesas_backend_api_net_core.Domain.Entities;

namespace despesas_backend_api_net_core.Domain.VM
{
    public class CategoriaVM : BaseModel
    {
        public string Descricao { get; set; }
        public int IdUsuario { get; set; }
        public int IdTipoCategoria { get; set; }
        internal virtual TipoCategoria TipoCategoria { get { return (TipoCategoria)IdTipoCategoria; } set   {  IdTipoCategoria = (int)value;  } }
    }
}