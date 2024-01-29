using FindexMapper.Core.Base;
using FindexMapper.Core.Enum;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Integrador.Models.Base
{
    public class BaseRequest
    {
        [JsonProperty("Pccodcta")]
        public string Pccodcta { get; set; }

        [JsonProperty("Pccodcli")]
        public string Pccodcli { get; set; }

        [JsonProperty("Pcnomcli")]
        public string Pcnomcli { get; set; }

        [JsonProperty("Pdfecsis")]
        public DateTime Pdfecsis { get; set; }

        [JsonProperty("Pdfecpro")]
        public DateTime Pdfecpro { get; set; }

        [JsonProperty("phora")]
        public string Phora { get; set; }

        [JsonProperty("Pdfecmod")]
        public DateTime Pdfecmod { get; set; }

        [JsonProperty("Pcoficina")]
        public string Pcoficina { get; set; }

        [JsonProperty("cnomofi")]
        public string Cnomofi { get; set; }

        [JsonProperty("PCtippag")]
        public string PCtippag { get; set; }

        [JsonProperty("Descrip")]
        public string Descrip { get; set; }

        [JsonProperty("Pcnrodoc")]
        public string Pcnrodoc { get; set; }

        [JsonProperty("Pcodfon")]
        public string Pcodfon { get; set; }

        [JsonProperty("cDescripcion")]
        public string CDescripcion { get; set; }

        [JsonProperty("venta")]
        public string Venta { get; set; }

        [JsonProperty("Pfondos")]
        public string Pfondos { get; set; }

        [JsonProperty("Pcnuming")]
        public string Pcnuming { get; set; }

        [JsonProperty("Pccoddom")]
        public string Pccoddom { get; set; }

        [JsonProperty("Pmunicipio")]
        public string Pmunicipio { get; set; }

        [JsonProperty("municipio")]
        public string Municipio { get; set; }

        [JsonProperty("Pdepartamento")]
        public string Pdepartamento { get; set; }

        [JsonProperty("cdeszon")]
        public string Cdeszon { get; set; }

        [JsonProperty("Pctelfam")]
        public string Pctelfam { get; set; }

        [JsonProperty("Pcteldom")]
        public string Pcteldom { get; set; }

        [JsonProperty("Pcorreo")]
        public string Pcorreo { get; set; }

        [JsonProperty("Pcnudoci")]
        public string Pcnudoci { get; set; }

        [JsonProperty("Pcnudotr")]
        public string Pcnudotr { get; set; }

        [JsonProperty("Pccodact")]
        public string Pccodact { get; set; }

        [JsonProperty("Pactidad")]
        public string Pactidad { get; set; }

        [JsonProperty("cdescri")]
        public string Cdescri { get; set; }

        [JsonProperty("Pcestadopag")]
        public string Pcestadopag { get; set; }

        [JsonProperty("Pccodusu")]
        public string Pccodusu { get; set; }

        [JsonProperty("Pnomusu")]
        public string Pnomusu { get; set; }

        [JsonProperty("Pccodana")]
        public string Pccodana { get; set; }

        [JsonProperty("Pmoninn")]
        public decimal Pmoninn { get; set; }

        [JsonProperty("Pmoninm")]
        public decimal Pmoninm { get; set; }

        [JsonProperty("Pmonseg")]
        public decimal Pmonseg { get; set; }

        [JsonProperty("Pvalor_letras")]
        public string PvalorLetras { get; set; }

        [JsonProperty("Poperaciones_varias")]
        public string PoperacionesVarias { get; set; }

        [JsonProperty("Pno_sujetas")]
        public decimal PnoSujetas { get; set; }

        [JsonProperty("Pexcentas")]
        public decimal Pexcentas { get; set; }

        [JsonProperty("Pgravadas")]
        public decimal Pgravadas { get; set; }

        [JsonProperty("Ptotal_factura")]
        public decimal PtotalFactura { get; set; }

        [JsonProperty("Pfactura_interna")]
        public string PfacturaInterna { get; set; }

        [JsonProperty("Ptipo_documento")]
        public string PtipoDocumento { get; set; }

        [JsonProperty("PCorrelativo")]
        public int PCorrelativo { get; set; }

        [JsonProperty("Pnumfac")]
        public string Pnumfac { get; set; }

        [JsonProperty("Pqr")]
        public string Pqr { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("sucursal")]
        public string Sucursal { get; set; }

        [JsonProperty("sucursalDepto")]
        public string SucursalDepto { get; set; }

        [JsonProperty("SucursalMun")]
        public string SucursalMun { get; set; }

        [JsonProperty("formaPago")]
        public string FormaPago { get; set; }

        [JsonProperty("tipoPago")]
        public string TipoPago { get; set; }

        [JsonProperty("cantidad")]
        public string Cantidad { get; set; }

        [JsonProperty("tipoItem")]
        public string TipoItem { get; set; }

        [JsonProperty("conceptoPmoninn")]
        public string ConceptoPmoninn { get; set; }

        [JsonProperty("conceptoPmoninm")]
        public string ConceptoPmoninm { get; set; }

        [JsonProperty("NRC")]
        public string NRC { get; set; }


        public Address? CreateReceiverAddress()
        {
            if (string.IsNullOrWhiteSpace(this.Pdepartamento) || string.IsNullOrWhiteSpace(this.Pmunicipio)) return null;

            return new Address()
            {
                Complement = $"{this.Pdepartamento} {this.Pmunicipio} {this.Cdeszon}",
                Department = this.Pdepartamento,
                Municipality = this.Pmunicipio
            };
        }

        public virtual (IdentificationDocumentType?, string?) HandleDocumentNumber()
        {
            Regex nitRegex = new("^([0-9]{14}|[0-9]{9})$");
            if (!string.IsNullOrWhiteSpace(Pcnudotr) && nitRegex.IsMatch(Pcnudotr)) return (IdentificationDocumentType.NIT, Pcnudotr);

            Regex duiRegex = new("^[0-9]{9}|[0-9]{8}-[0-9]{1}$");
            if (!string.IsNullOrWhiteSpace(Pcnudoci) && duiRegex.IsMatch(Pcnudoci)) return (IdentificationDocumentType.DUI, Pcnudoci);

            if (!string.IsNullOrWhiteSpace(Pcnudotr) || !string.IsNullOrWhiteSpace(Pcnudoci)) return (IdentificationDocumentType.Other, Pcnudotr ?? Pcnudoci);

            return (null, null);
        }

    }
}
