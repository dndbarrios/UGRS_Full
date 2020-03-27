
namespace Core.Utility
{
    public class StaticSessionUtility
    {
        public static object mObjSeccion;

        public static void SetCurrentSession(object pObjSession)
        {
            mObjSeccion = pObjSession;
        }

        public static object GetCurrentSession()
        {
            return mObjSeccion;
        }
    }
}
