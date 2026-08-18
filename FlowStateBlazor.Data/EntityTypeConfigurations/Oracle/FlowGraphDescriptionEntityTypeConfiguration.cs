using FlowStateBlazor.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowStateBlazor.Data.EntityTypeConfigurations.Oracle
{
    public class FlowGraphDescriptionEntityTypeConfiguration : IEntityTypeConfiguration<FlowGraphDescription>
    {
        public void Configure(EntityTypeBuilder<FlowGraphDescription> builder)
        {
            builder.ToTable("FLOW_FLOWGRAPH_DESCRIPTION");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasColumnName("ID")
                .ValueGeneratedOnAdd();

            builder.Property(e => e.Name)
                .HasColumnName("NAME")
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("NVARCHAR2(255)");

            builder.Property(e => e.Description)
                .HasColumnName("DESCRIPTION")
                .HasMaxLength(255)
                .HasColumnType("NVARCHAR2(255)");

            builder.Property(e => e.JsonFlowSerialized)
                .HasColumnName("JSON_SERIALIZED_FLOW")
                .IsRequired()
                .HasColumnType("CLOB");
        }
    }
}
