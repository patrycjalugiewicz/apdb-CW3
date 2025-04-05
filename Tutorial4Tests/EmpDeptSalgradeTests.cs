using System.Diagnostics.Tracing;
using Tutorial3.Models;

public class EmpDeptSalgradeTests
{
    public static List<Emp> GetSalesmen()
    {
        var emps = Database.GetEmps();
        var result = emps.Where(e => e.Job == "SALESMAN").ToList();
        return result;
    }
    
    [Fact]
    public void ShouldReturnAllSalesmen()
    {
        var emps = Database.GetEmps();

        List<Emp> result = GetSalesmen();

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal("SALESMAN", e.Job));
    }
    
    public static List<Emp> GetDeptNo30Desc()
    {
        var emps = Database.GetEmps();
        var result = emps.Where(e => e.DeptNo == 30).OrderByDescending(e => e.Sal).ToList();
        return result;
    }
    
    [Fact]
    public void ShouldReturnDept30EmpsOrderedBySalaryDesc()
    {
        var emps = Database.GetEmps();

        List<Emp> result = GetDeptNo30Desc(); 

        Assert.Equal(2, result.Count);
        Assert.True(result[0].Sal >= result[1].Sal);
    }
    
    public static List<Emp> GetEmpInChicago()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();
        var chicagoDepts = depts.Where(d => d.Loc == "Chicago").Select(d => d.DeptNo);
        var result = emps.Where(e => chicagoDepts.Contains(e.DeptNo)).ToList();
        return result;
    }

    [Fact]
    public void ShouldReturnEmployeesFromChicago()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();

        List<Emp> result = GetEmpInChicago(); 

        Assert.All(result, e => Assert.Equal(30, e.DeptNo));
    }

    public static List<Emp> GetNamesSal()
    {
        var emps = Database.GetEmps();
        var result = emps.Select(e => new Emp
        {
            EName = e.EName,
            Sal = e.Sal
        }).ToList();
        return result;
    }
    
    [Fact]
    public void ShouldSelectNamesAndSalaries()
    {
        var emps = Database.GetEmps();

        var result = GetNamesSal(); 
        
         Assert.All(result, r =>
         {
            Assert.False(string.IsNullOrWhiteSpace(r.EName));
             Assert.True(r.Sal > 0);
         });
    }
    
    public static List<(string EName, string DName)> GetEmpDeptName()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();
        var join = emps.Join(depts, e => e.DeptNo, d => d.DeptNo, (e,d) => (e.EName, d.DName)).ToList();
        return join;
    }
    
    [Fact]
    public void ShouldJoinEmployeesWithDepartments()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();

        var result = GetEmpDeptName(); 

        Assert.Contains(result, r => r.DName == "SALES" && r.EName == "ALLEN");
    }
    
    public static List<(int DeptNo, int Count)> GetEmpDeptCount()
    {
        var emps = Database.GetEmps();
        var result = emps.GroupBy(emp => emp.DeptNo).Select(group => (DeptNo: group.Key, EmployeeCount: group.Count())).ToList();
        return result;
    }
    
    [Fact]
    public void ShouldCountEmployeesPerDepartment()
    {
        var emps = Database.GetEmps();

         var result = GetEmpDeptCount(); 
        
         Assert.Contains(result, g => g.DeptNo == 30 && g.Count == 2);
    }
    
    public static List<(string EName, decimal? Comm)> GetEmpWithComm()
    {
        var emps = Database.GetEmps();
        var result = emps.Where(e => e.Comm != null).Select(e => (e.EName, e.Comm)).ToList();
        return result;
    }
    
    [Fact]
    public void ShouldReturnEmployeesWithCommission()
    {
        var emps = Database.GetEmps();

         var result = GetEmpWithComm(); 
        
         Assert.All(result, r => Assert.NotNull(r.Comm));
    }
    
    public static List<(string EName, int Grade)> GetEnameGrade()
    {
        var emps = Database.GetEmps();
        var grades = Database.GetSalgrades();
        var result = emps.SelectMany(emp => grades
                .Where(grade => emp.Sal >= grade.Losal && emp.Sal <= grade.Hisal)
                .Select(grades => (emp.EName, grades.Grade)))
            .ToList();
        return result;
    }
    
    [Fact]
    public void ShouldMatchEmployeeToSalaryGrade()
    {
        var emps = Database.GetEmps();
        var grades = Database.GetSalgrades();

         var result = GetEnameGrade();
        
         Assert.Contains(result, r => r.EName == "ALLEN" && r.Grade == 3);
    }
    
    public static List<(int DeptNo, decimal AvgSal)> GetAvgSalByDepartment()
    {
       
        var emps = Database.GetEmps();
        var result = emps.GroupBy(emp => emp.DeptNo)
            .Select(group => (DeptNo: group.Key, AvgSal: group.Average(emp => emp.Sal))).ToList();
        return result;
    }
    
    [Fact]
    public void ShouldCalculateAverageSalaryPerDept()
    {
        var emps = Database.GetEmps();

        var result = GetAvgSalByDepartment(); 
        
         Assert.Contains(result, r => r.DeptNo == 30 && r.AvgSal > 1000);
    }
    public static List<string> GetEmpBetterSal()
    {
        var emps = Database.GetEmps();
        var result = emps.Where(emp =>
                emp.Sal > emps.Where(e => e.DeptNo == emp.DeptNo).Average(e => e.Sal))
            .Select(emp => emp.EName)
            .ToList();

        return result;
    }
    
    [Fact]
    public void ShouldReturnEmployeesEarningMoreThanDeptAverage()
    {
        var emps = Database.GetEmps();

        var result = GetEmpBetterSal(); 
        
         Assert.Contains("ALLEN", result);
    }
}
