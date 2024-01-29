using FindexMapper.Core.Entities;
using FindexMapper.Service.Interfaces;
using Integrador.Mappers.Invoices;
using Integrador.Models.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Integrador.Services
{
    public class ParseService : IParseService
    {
        private readonly IInvoiceMapper _invoiceMapper;

        public ParseService(IInvoiceMapper invoiceMapper)
        {
            _invoiceMapper = invoiceMapper ?? throw new ArgumentNullException(nameof(invoiceMapper));
        }

        public object? Parse(JToken resource, FindexMapper.Service.Enum.Environment environment)
        {
            try
            {
                var baseRequest = Deserialize(resource);

                // Accede a las propiedades de BaseRequest directamente
                var documentType = baseRequest.PtipoDocumento; // Ajusta según la estructura real de tu clase BaseRequest
                var isInvoice = documentType == "01";

                if (isInvoice)
                {
                    // Imprime o registra las propiedades relevantes para verificar
                    Console.WriteLine($"BaseRequest: {JsonConvert.SerializeObject(baseRequest)}");

                    return _invoiceMapper.Map(baseRequest, environment);
                }
            }
            catch (Exception ex)
            {
                // Registra o maneja la excepción según tus necesidades
                Console.WriteLine($"Error durante el proceso de análisis: {ex.Message}");
                throw;
            }

            throw new InvalidOperationException("El tipo de DTE no es soportado");
        }

        private static BaseRequest Deserialize(JToken resource)
        {
            return resource.ToObject<BaseRequest>() ?? new BaseRequest();
        }

    }
}
