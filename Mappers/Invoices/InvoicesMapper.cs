using FindexMapper.Core.Base;
using FindexMapper.Core.Entities;
using FindexMapper.Core.Enum;
using FindexMapper.Core.Utils;
using FindexMapper.Service.Interfaces;
using FindexMapper.Service.Services.ControlNumbers;
using Integrador.Models;
using Integrador.Models.Base;
using Integrador.Utils;
using Microsoft.Extensions.Options;

namespace Integrador.Mappers.Invoices;

public class InvoiceMapper : IInvoiceMapper
{
    private readonly IControlNumberService _controlNumberService;
    private readonly ISourceProvider _sourceProvider;
    private readonly SenderInfo _senderInfo;

    public InvoiceMapper(IControlNumberService controlNumberService, IOptions<SenderInfo> options, ISourceProvider sourceProvider)
    {
        _controlNumberService = controlNumberService;
        _senderInfo = options.Value;
        _sourceProvider = sourceProvider;
    }

    public Invoice Map(BaseRequest? input, FindexMapper.Service.Enum.Environment environment)
    {
        try
        {
            if (input is null) return new Invoice();
            var invoice = new Invoice()
            {
                Identification = CreateIdentification(input, environment),
                Sender = CreateSender(),
                Receiver = CreateReceiver(input),
                DocumentBody = CreateDocumentBody(input),
                Summary = CreateSummary(input)
            };
            return invoice;
        }
        catch (Exception)
        {
            return new Invoice();
        }
    }

    private static FindexMapper.Core.Base.Invoice.Summary CreateSummary(BaseRequest? row)
    {
        try
        {
            if (row is null) return new FindexMapper.Core.Base.Invoice.Summary();
            var totalAmount = Math.Round(Convert.ToDecimal(row.PtotalFactura), 2);
            return new FindexMapper.Core.Base.Invoice.Summary()
            {
                DiscountPercentage = 0.00m,
                TotalDiscount = 0.00m,
                Subtotal = Math.Round(Convert.ToDecimal(row.Pexcentas), 2),
                TotalTaxed = row.Pgravadas,
                TotalExempt = row.Pexcentas,
                TotalNotSubject = row.PnoSujetas,
                Status = row.FormaPago?.ToEnum(FindexMapper.Core.Enum.OperationStatus.Cash) ?? FindexMapper.Core.Enum.OperationStatus.Cash,
                Payment = null,
                SubTotalSales = row.Pexcentas,
                TotalDescription = totalAmount.ToString("F").ToInvoiceFormat(true) ?? string.Empty,
                TotalAmount = totalAmount,
                TotalToPay = totalAmount,
                TotalVat = Math.Round(Convert.ToDecimal(row.PoperacionesVarias), 2)
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.Invoice.Summary();
        }
    }

    //private static ICollection<Payment>? CreatePayment(Oinv row)
    //{
    //    try
    //    {
    //        if (row.U_Formapago_FE != "2") return null;
    //        return new List<Payment>()
    //        {
    //            new Payment()
    //            {
    //                Amount = Math.Round(Convert.ToDecimal(row.DocTotal), 2),
    //                Code = row.U_MetodoPago_FE ?? string.Empty,
    //                Period = Convert.ToDecimal(row.U_TipoPlazo_FE),
    //                Timeframe = row.U_CantidadTiempo_FE
    //            }
    //        };
    //    }
    //    catch (Exception)
    //    {
    //        return null;
    //    }
    //}

    private static ICollection<FindexMapper.Core.Base.Invoice.DocumentBody> CreateDocumentBody(BaseRequest? baseRequest)
    {
        try
        {
            if (baseRequest is null) return Enumerable.Empty<FindexMapper.Core.Base.Invoice.DocumentBody>().ToList();
            int index = 1;
            var result = new List<FindexMapper.Core.Base.Invoice.DocumentBody>
        {
            new FindexMapper.Core.Base.Invoice.DocumentBody()
            {
                Number = index++,
                Description = $"{baseRequest?.ConceptoPmoninm} {(baseRequest?.ConceptoPmoninm is string ? (string?)baseRequest?.ConceptoPmoninm : string.Empty)}",
                Quantity = 1,
                Unitprice = Math.Round(Convert.ToDecimal(baseRequest?.Pmoninm), 2),
                Vat = 0.00m,
                UnitOfMeasurement = 99, //otro,
                Type = baseRequest?.TipoItem?.ToEnum(FindexMapper.Core.Enum.DocumentBodyType.Product) ?? FindexMapper.Core.Enum.DocumentBodyType.Product,
                TaxableSale = 0.00m
            }
        };
            return result;
        }
        catch (Exception)
        {
            return Enumerable.Empty<FindexMapper.Core.Base.Invoice.DocumentBody>().ToList();
        }
    }



    private FindexMapper.Core.Base.Invoice.Receiver? CreateReceiver(BaseRequest? row)
    {
        try
        {
            if (row is null) return null;
            var number = string.IsNullOrEmpty(row.Pcnudoci) ? row.Pcnudotr : row.Pcnudoci ?? null;
            var documentNumber = HandleDocumentNumber(null, number); //null? no hay un campo el numero de documento

            var economicActivity = _sourceProvider.Catalog(new
            {
                CatalogId = 19,
                Key = row.Pccodact
            });

            return new FindexMapper.Core.Base.Invoice.Receiver()
            {
                DocumentNumber = documentNumber.Item1,
                DocumentType = documentNumber.Item2,
                Name = !string.IsNullOrWhiteSpace(row.Pcnomcli) ? row.Pcnomcli : null,
                Address = row.CreateReceiverAddress(),
                EconomicActivity = economicActivity.Name,
                EconomicActivityCode = row.Pccodact,
                Email = row.Pcorreo,
                Nrc = StringUtils.RemoveGuionFromString(row.NRC)?.Replace("\\", "").Replace("/", "") ?? string.Empty,
                Phone = row.Pcteldom
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static (string?, FindexMapper.Core.Enum.IdentificationDocumentType?) HandleDocumentNumber(string? documentType, string? documentNumber)
    {
        try
        {
            if (string.IsNullOrEmpty(documentType) || string.IsNullOrEmpty(documentNumber)) return (null, null);
            if (documentType == "NA") return (null, null);
            var docType = documentType.ToEnum(FindexMapper.Core.Enum.IdentificationDocumentType.DUI);
            documentNumber = StringUtils.HandleDocumentNumber(docType, documentNumber);
            return (documentNumber, docType);
        }
        catch (Exception)
        {
            return (null, null);
        }
    }

    private FindexMapper.Core.Base.Invoice.Sender CreateSender()
    {
        try
        {
            return new FindexMapper.Core.Base.Invoice.Sender()
            {
                Address = new Address()
                {
                    Complement = _senderInfo.AddressComplement,
                    Department = _senderInfo.Department,
                    Municipality = _senderInfo.Municipality
                },
                EconomicActivity = _senderInfo.EconomicActivity,
                EconomicActivityCode = _senderInfo.EconomicActivityCode,
                Email = _senderInfo.Email,
                Establishment = FindexMapper.Core.Enum.EstablishmentType.Branch,
                Name = _senderInfo.Name,
                Nit = _senderInfo.NIT,
                Nrc = _senderInfo.NRC,
                Phone = _senderInfo.Phone,
                TradeName = _senderInfo.TradeName,
                EstablishmentCode = _senderInfo.EstablishmentCode,
                PointOfSaleCode = _senderInfo.PointOfSaleCode,
                MHEstablishmentCode = _senderInfo.MHEstablishmentCode,
                MHPointOfSaleCode = _senderInfo.MHPointOfSaleCode
            };
        }
        catch (Exception)
        {
            return new FindexMapper.Core.Base.Invoice.Sender();
        }
    }

    private Identification CreateIdentification(BaseRequest? row, FindexMapper.Service.Enum.Environment environment)
    {
        try
        {
            if (row is null) return new Identification();
            return new Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.Invoice,
                IssueDate = row.Pdfecmod,
                IssueTime = row.Pdfecmod.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.ConsumidorFinalJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = row.Id
            };
        }
        catch (Exception)
        {
            return new Identification()
            {
                Type = FindexMapper.Core.Enum.DocumentType.Invoice,
                IssueDate = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime,
                IssueTime = DateTimeOffset.Now.ToOffset(new TimeSpan(-6, 0, 0)).DateTime.ToString("HH:mm:ss"),
                Model = FindexMapper.Core.Enum.ModelType.Normal,
                Operation = FindexMapper.Core.Enum.OperationType.Normal,
                Version = FindexMapper.Core.Constants.ConsumidorFinalJsonSchemaVersion,
                Environment = (FindexMapper.Core.Enum.Environment)environment,
                Identifier = Guid.NewGuid().ToString("D").ToUpperInvariant(),
                ControlNumber = ""
            };
        }
    }
}