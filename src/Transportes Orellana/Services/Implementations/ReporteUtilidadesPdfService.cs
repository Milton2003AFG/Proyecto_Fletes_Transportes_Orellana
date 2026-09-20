using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Transportes_Orellana.Models.ViewModels;
using Transportes_Orellana.Services.Interfaces;

namespace Transportes_Orellana.Services.Implementations;

public class ReporteUtilidadesPdfService : IReporteUtilidadesPdfService
{
    public byte[] GenerarReporte(CalculoUtilidadViewModel utilidad)
    {
        var documento = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);

                page.Header()
                    .Column(column =>
                    {
                        column.Item()
                            .Text("TRANSPORTES ORELLANA")
                            .FontSize(18)
                            .Bold();

                        column.Item()
                            .Text($"Reporte de utilidad - Flete #{utilidad.FleteId}")
                            .FontSize(14);

                        column.Item()
                            .PaddingTop(5)
                            .Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}")
                            .FontSize(9);
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(15);

                        // Datos del flete
                        column.Item()
                            .Text("Datos del flete")
                            .FontSize(13)
                            .Bold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            AgregarDato(table, "Cliente", utilidad.Cliente);
                            AgregarDato(table, "Motorista", utilidad.Motorista);
                            AgregarDato(table, "Unidad", utilidad.Unidad);
                            AgregarDato(table, "Origen", utilidad.LugarRecolecta);
                            AgregarDato(table, "Destino", utilidad.LugarEntrega);
                            AgregarDato(table, "Estado", utilidad.Estado);
                        });

                        // Resumen financiero
                        column.Item()
                            .PaddingTop(10)
                            .Text("Resumen financiero")
                            .FontSize(13)
                            .Bold();

                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(120);
                            });

                            AgregarMonto(table, "Valor del flete", utilidad.ValorFlete);
                            AgregarMonto(table, "Gastos del camión", utilidad.GastosCamion);
                            AgregarMonto(table, "Gastos varios", utilidad.GastosVarios);
                            AgregarMonto(table, "Gastos de producción", utilidad.GastosProduccion);
                            AgregarMonto(table, "Total de gastos", utilidad.TotalGastos);
                            AgregarMonto(table, "Utilidad neta", utilidad.UtilidadNeta);

                            table.Cell()
                                .BorderBottom(1)
                                .Padding(5)
                                .Text("Rentabilidad")
                                .Bold();

                            table.Cell()
                                .BorderBottom(1)
                                .Padding(5)
                                .AlignRight()
                                .Text($"{utilidad.PorcentajeRentabilidad:N2}%")
                                .Bold();
                        });

                        // Gastos individuales
                        column.Item()
                            .PaddingTop(10)
                            .Text("Detalle de gastos")
                            .FontSize(13)
                            .Bold();

                        if (utilidad.Gastos.Count == 0)
                        {
                            column.Item()
                                .Text("No hay gastos registrados para este flete.");
                        }
                        else
                        {
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn(2);
                                    columns.ConstantColumn(90);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(Encabezado).Text("Grupo");
                                    header.Cell().Element(Encabezado).Text("Tipo");
                                    header.Cell().Element(Encabezado).Text("Concepto");
                                    header.Cell().Element(Encabezado).Text("Monto");
                                });

                                foreach (var gasto in utilidad.Gastos)
                                {
                                    table.Cell().Element(Celda).Text(gasto.Grupo);
                                    table.Cell().Element(Celda).Text(gasto.TipoGasto);
                                    table.Cell().Element(Celda).Text(gasto.Concepto);

                                    table.Cell()
                                        .Element(Celda)
                                        .AlignRight()
                                        .Text($"${gasto.Monto:N2}");
                                }
                            });
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Transportes Orellana - Página ");
                        text.CurrentPageNumber();
                    });
            });
        });

        return documento.GeneratePdf();
    }

    private static void AgregarDato(
        TableDescriptor table,
        string titulo,
        string valor)
    {
        table.Cell()
            .Padding(5)
            .Text(titulo)
            .Bold();

        table.Cell()
            .Padding(5)
            .Text(valor);
    }

    private static void AgregarMonto(
        TableDescriptor table,
        string titulo,
        decimal monto)
    {
        table.Cell()
            .BorderBottom(1)
            .Padding(5)
            .Text(titulo);

        table.Cell()
            .BorderBottom(1)
            .Padding(5)
            .AlignRight()
            .Text($"${monto:N2}");
    }

    private static IContainer Encabezado(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten3)
            .BorderBottom(1)
            .Padding(5);
    }

    private static IContainer Celda(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5);
    }
}