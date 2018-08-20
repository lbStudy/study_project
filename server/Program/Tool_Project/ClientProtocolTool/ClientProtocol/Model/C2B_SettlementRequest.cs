using System.Collections.Generic;

namespace Model
{
	public class C2B_SettlementRequest : Request
    {
        public long roomid;
        public List<byte> shang;
        public List<byte> zhong;
        public List<byte> xia;
	}
}
