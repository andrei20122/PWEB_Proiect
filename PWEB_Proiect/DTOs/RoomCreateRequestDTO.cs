namespace PWEB_Proiect.DTOs
{
    public class RoomCreateRequestDTO
    {
        public int Capacity { get; set; }
        public string Building { get; set; }
        public int Floor { get; set; }
        public float MonthlyCost { get; set; }
        public int NrRoom { get; set; }
    }
}
