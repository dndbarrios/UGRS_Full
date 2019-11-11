namespace UGRS.CheckAdminSDK.SDK.DI.DAO.Interfaces {
    public interface ITableDAO<T> where T : ITable {

        int Add(T pObjRecord);
        int Update(T pObjRecord, int key);
    }
}
