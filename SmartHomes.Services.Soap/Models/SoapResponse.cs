using System.Runtime.Serialization;

namespace SmartHomes.Services.Soap.Models
{
    public class SoapResponse<T>
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem descritiva do resultado
        /// </summary>
        [DataMember]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dados retornados pela operação
        /// </summary>
        [DataMember]
        public T? Data { get; set; }
    }
}
