namespace PAD.Models.ChartFields
{
    public class DeptID : ChartField
    {
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
