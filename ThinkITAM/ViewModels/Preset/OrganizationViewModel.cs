using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.Preset;
internal class OrganizationViewModel
{
    //索引
    public float Index
    {
        get; set;
    }


    //组织
    public string Organization
    {
        get; set;
    }

    //部门
    public string Department
    {
        get; set;
    }

    //备注
    public string Note
    {
        get; set;
    }
}

internal class OrganizationOneViewModel
{
    //索引
    public int Index
    {
        get; set;
    }


    //组织
    public string Organization
    {
        get; set;
    }

}

internal class DepartmentViewModel
{
    //索引
    public int Index
    {
        get; set;
    }


    //组织
    public string Department
    {
        get; set;
    }

}
internal class GroupViewModel
{
    //索引
    public int Index
    {
        get; set;
    }


    //组织
    public string Group
    {
        get; set;
    }

}