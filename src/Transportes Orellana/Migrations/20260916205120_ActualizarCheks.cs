using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transportes_Orellana.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarCheks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "unidad_transporte",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "unidad_transporte",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "activo",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "motorista",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "motorista",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "activo",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "gasto_flete",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "flete",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "flete",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "programado",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "cliente",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "cliente",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "activo",
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddCheckConstraint(
                name: "chk_unidad_estado",
                table: "unidad_transporte",
                sql: "estado IN ('activo', 'inactivo')");

            migrationBuilder.AddCheckConstraint(
                name: "chk_motorista_estado",
                table: "motorista",
                sql: "estado IN ('activo', 'inactivo')");

            migrationBuilder.AddCheckConstraint(
                name: "chk_gasto_tipo",
                table: "gasto_flete",
                sql: "tipo_gasto IN ('camion', 'varios', 'produccion')");

            migrationBuilder.AddCheckConstraint(
                name: "chk_flete_estado",
                table: "flete",
                sql: "estado IN ('programado', 'en_proceso', 'terminado', 'con_devolucion', 'con_queja')");

            migrationBuilder.AddCheckConstraint(
                name: "chk_cliente_estado",
                table: "cliente",
                sql: "estado IN ('activo', 'inactivo')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "chk_unidad_estado",
                table: "unidad_transporte");

            migrationBuilder.DropCheckConstraint(
                name: "chk_motorista_estado",
                table: "motorista");

            migrationBuilder.DropCheckConstraint(
                name: "chk_gasto_tipo",
                table: "gasto_flete");

            migrationBuilder.DropCheckConstraint(
                name: "chk_flete_estado",
                table: "flete");

            migrationBuilder.DropCheckConstraint(
                name: "chk_cliente_estado",
                table: "cliente");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "unidad_transporte",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "unidad_transporte",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "activo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "motorista",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "motorista",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "activo");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "gasto_flete",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "flete",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "flete",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "programado");

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha_registro",
                table: "cliente",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<string>(
                name: "estado",
                table: "cliente",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldDefaultValue: "activo");
        }
    }
}
