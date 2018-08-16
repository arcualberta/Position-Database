namespace PD.Models.ChartFields
{
    public class DeptID : ChartField
    {
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
    }
}
