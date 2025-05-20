using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThinkITAM.ViewModels.DatabaseEntity.Asset;
public class AssetViewModel
{
    public int Id
    {
        get; set;
    }

    public string AssetId
    {
        get; set;
    }

    public string AssetQrCode
    {
        get; set;
    }

    public string AssetType
    {
        get; set;
    }

    public string DeviceType
    {
        get; set;
    }

    public string AssetTag
    {
        get; set;
    }

    public int AssetNumber
    {
        get; set;
    }

    public string PurchaseDate
    {
        get; set;
    }

    public string PurchasePrice { get; set; }

    public string Manufacturer
    {
        get; set;
    }

    public string Model
    {
        get; set;
    }

    public string SerialNumber { get; set; }


    public string Configuration { get; set; }

    public string Location
    {
        get; set;
    }

    public string UserOrganization { get; set; }

    public string UserDepartment
    {
        get; set;
    }

    public string UserGroup     {
        get; set;
    }

    public string User
    {
        get;set;
    }

    public string UserPhone
    {
        get; set;
    }

    public string Consumer
    {
        get;set;
    }

    public string AssetStatus
    {
        get; set;
    }

    public string UsedYear
    {
        get;
        set;
    }

    public string ScrapDate
    {
        get;set;
    }

    public string Notes { get; set; }

    public string TagA { get; set; }
    public string TagB { get; set; }
    public string TagC { get; set; }
    public string TagD { get; set; }
    public string TagE { get; set; }
    public string TagF { get; set; }
}
