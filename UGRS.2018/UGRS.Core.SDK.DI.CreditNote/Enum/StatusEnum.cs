using System.ComponentModel;

namespace UGRS.Core.SDK.DI.CreditNote.Enum
{
    public enum StatusEnum : int
    {
        [Description("Cancelado")]//U_IsCanceled = Y
        Canceled = 0,

        [Description("Por generar reporte")] //Sin guardar
        PendingReport = 1,

        [Description("Pendiente de autorizar")] //Generado en UDT 
        PendingAutorized = 2,

        [Description("Autorizado")] //U_IsAutorized Y
        Authorized = 3,

        [Description("Procesado")] //U_IsProcessed = Y
        Processed = 4,

        [Description("Pendiente actualizar documentos relacionados")] //List U_IsDocRel = Y con algunos N
        PendignDocRel = 5,

        [Description("Documentos relacionados ok")] //List U_IsDocRel = Y
        DocRelOk = 6,

        [Description("Pendiente Nota de credito")] //List U_IsDocument = Y con algunos N
        PendignNC = 7,

        [Description("Nota de credito ok")] //List U_IsDocument = Y
        NcOk = 8,

        
    }
}
