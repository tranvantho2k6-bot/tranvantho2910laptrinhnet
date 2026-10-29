using System.Data;
using CafePOS.DAL;
namespace CafePOS.BUS;
public static class BanAnBUS
{
    public static DataTable GetAll() => BanAnDAL.GetAll();
}
