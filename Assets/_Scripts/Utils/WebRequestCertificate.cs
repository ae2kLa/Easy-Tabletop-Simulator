using UnityEngine.Networking;

namespace Tabletop
{
    public class WebRequestCertificate : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //return base.ValidateCertificate(certificateData);
            return true;
        }
    }
}
