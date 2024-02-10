namespace AAgIO.Models
{
    public class RadioChannel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Frequency { get; set; }
        public string Location { get; set; }
     
        public RadioChannel(int id , string name, string frequency, string location)
        {
		    Id = id;
		    Name = name;
		    Frequency = frequency;
		    Location = location;
	    }
	}
}	    
