namespace Tutorial3Tests;
using Tutorial3.Models;

public class AdvancedEmpDeptTests
{
    public static decimal GetMaxSalary()
    {
        var emps = Database.GetEmps();
        return emps.Max(emp => emp.Sal);
    }
    [Fact]
    public void ShouldReturnMaxSalary()
    {
        var emps = Database.GetEmps();

        decimal? maxSalary = GetMaxSalary(); 

        Assert.Equal(5000, maxSalary);
    }

    public static decimal GetMinSalDept30()
    {
        var emps = Database.GetEmps();
        return emps.Where(emp => emp.DeptNo == 30).Min(emp => emp.Sal);
    }
    [Fact]
    public void ShouldReturnMinSalaryInDept30()
    {
        var emps = Database.GetEmps();

        decimal? minSalary = GetMinSalDept30();

        Assert.Equal(1250, minSalary);
    }

    public static List<Emp> GetFirstTwoEmp()
    {
        var emps = Database.GetEmps();
        return emps.OrderBy(emp => emp.HireDate).Take(2).ToList();
    }
    
    [Fact]
    public void ShouldReturnFirstTwoHiredEmployees()
    {
        var emps = Database.GetEmps();

        var firstTwo = GetFirstTwoEmp(); 
        
         Assert.Equal(2, firstTwo.Count);
         Assert.True(firstTwo[0].HireDate <= firstTwo[1].HireDate);
    }
    
    public static List<string> GetDisJob()
    {
        var emps = Database.GetEmps();
        return emps.Select(emp => emp.Job).Distinct().ToList();
    }
    
    [Fact]
    public void ShouldReturnDistinctJobTitles()
    {
        var emps = Database.GetEmps();

        var jobs = GetDisJob(); 
        
        Assert.Contains("PRESIDENT", jobs);
        Assert.Contains("SALESMAN", jobs);
    }
    
    public static List<Emp> GetEmpWithMan()
    {
        var emps = Database.GetEmps();
        return emps.Where(emp => emp.Mgr != null).ToList();
    }
    
    [Fact]
    public void ShouldReturnEmployeesWithManagers()
    {
        var emps = Database.GetEmps();

        var withMgr = GetEmpWithMan(); 
        
        Assert.All(withMgr, e => Assert.NotNull(e.Mgr));
    }
    public static List<Emp> GetEmplSalMoreThan500()
    {
        var emps = Database.GetEmps();
        return emps.Where(emp => emp.Sal > 500).ToList();
    }
    
    [Fact]
    public void AllEmployeesShouldEarnMoreThan500()
    {
        var emps = Database.GetEmps();
        var result = GetEmplSalMoreThan500(); 
        Assert.Equal(emps.Count, result.Count);
    }
    
    public static List<Emp> GetEmpCommOver400()
    {
        var emps = Database.GetEmps();
        return emps.Where(emp => emp.Comm != null && emp.Comm > 400).ToList();
    }
    [Fact]
    public void ShouldFindAnyWithCommissionOver400()
    {
        var emps = Database.GetEmps();

        var result = GetEmpCommOver400(); 
        Assert.Equal(emps.Count(emp => emp.Comm != null && emp.Comm >400), result.Count);
    }
    public static List<(string Employee, string Manager)> GetEmpManPairs()
    {
        var emps = Database.GetEmps();
        return emps.Join(emps,
                emp => emp.Mgr,
                mgr => mgr.EmpNo,
                (emp, mgr) => (Employee: emp.EName, Manager: mgr.EName))
            .ToList();
    }
    
    [Fact]
    public void ShouldReturnEmployeeManagerPairs()
    {
        var result = GetEmpManPairs();
        Assert.Contains(result, r => r.Employee == "SMITH" && r.Manager == "FORD");
    }
    public static List<(string EName, decimal Total)> GetIncomeEmp()
    {
        var emps = Database.GetEmps();
        return emps.Select(emp => (emp.EName, TotalIncome: emp.Sal + (emp.Comm ?? 0))).ToList();
    }
    [Fact]
    public void ShouldReturnTotalIncomeIncludingCommission()
    {
        var emps = Database.GetEmps();
        var result = GetIncomeEmp(); 
        Assert.Contains(result, r => r.EName == "ALLEN" && r.Total == 1900);
    }
    public static List<(string EName, string DName, int Grade)> GetEmpDeptGrade()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();
        var grades = Database.GetSalgrades();

        var empDeptJoin = emps.Join(depts,
            e => e.DeptNo,
            d => d.DeptNo,
            (e, d) => new { e, d });
        
        var result = empDeptJoin.SelectMany(
                combined => grades.Where(s => combined.e.Sal >= s.Losal && combined.e.Sal <= s.Hisal),
                (combined, salgrade) => new { combined.e.EName, combined.d.DName, salgrade.Grade })
            .Select(joined => (joined.EName, joined.DName, joined.Grade))
            .ToList();

        return result;
    }
    
    [Fact]
    public void ShouldJoinEmpDeptSalgrade()
    {
        var emps = Database.GetEmps();
        var depts = Database.GetDepts();
        var grades = Database.GetSalgrades();
        var result = GetEmpDeptGrade(); 
        Assert.Contains(result, r => r.EName == "ALLEN" && r.DName == "SALES" && r.Grade == 3);
    }
}
