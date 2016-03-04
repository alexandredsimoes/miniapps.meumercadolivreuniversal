namespace MyML.UWP.Aggregattor
{
    public class MessengerDetails
    {
        public object Id { get; set; }
        public object SubId { get; set; }
        public SourceDetail Source { get; set; }
    }
    public enum SourceDetail
    {
        sdSellAnswer = 1
    }
}