namespace WebAPIAutores.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        private int RecordsPorPagina { get; set; } = 10;
        private readonly int RecordsMaximosPorPagina = 50;
        public int RecordsXPagina { 
            get {
                return RecordsPorPagina;
            } 
            set { 
                RecordsPorPagina = (value > RecordsMaximosPorPagina) ? RecordsMaximosPorPagina: value;
            } 
        }
    }
}
