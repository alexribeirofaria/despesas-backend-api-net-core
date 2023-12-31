﻿using despesas_backend_api_net_core.Domain.Entities;
using despesas_backend_api_net_core.Domain.VM;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace despesas_backend_api_net_core.Infrastructure.Data.EntityConfig
{
    public class CategoriaMap : IParser<CategoriaVM, Categoria>, IParser<Categoria, CategoriaVM>, IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Descricao)
            .IsRequired(false)
            .HasMaxLength(100);

            builder.Property(m => m.UsuarioId)
            .IsRequired();

            builder.Property(m => m.TipoCategoria)
            .IsRequired();
        }

        public Categoria Parse(CategoriaVM origin)
        {
            if (origin == null) return new Categoria();
            return new Categoria
            {
                Id = origin.Id,
                Descricao = origin.Descricao,
                TipoCategoria = origin.IdTipoCategoria == 1 ? TipoCategoria.Despesa : TipoCategoria.Receita,
                UsuarioId = origin.IdUsuario
            };
        }

        public CategoriaVM Parse(Categoria origin)
        {
            if (origin == null) return new CategoriaVM();
            return new CategoriaVM
            {
                Id = origin.Id,
                Descricao = origin.Descricao,
                IdTipoCategoria = (int)origin.TipoCategoria,
                IdUsuario = origin.UsuarioId                
            };
        }

        public List<Categoria> ParseList(List<CategoriaVM> origin)
        {
            if (origin == null) return new List<Categoria>();
            return origin.Select(item => Parse(item)).ToList();
        }

        public List<CategoriaVM> ParseList(List<Categoria> origin)
        {
            if (origin == null) return new List<CategoriaVM>();
            return origin.Select(item => Parse(item)).ToList();
        }
    }
}
