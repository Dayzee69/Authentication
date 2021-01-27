
using System;

namespace Authentication
{
    public class Subject
    {
        public int id { get; set; }
        public string email { get; set; }
        public string personal_mobile { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string second_name { get; set; }
        public string p_serie { get; set; }
        public string p_number { get; set; }
        public string personal_birthday { get; set; }
        public string p_date { get; set; }
        public string p_who { get; set; }
        public string personal_zip { get; set; }
        public string doc_type { get; set; }
        public string doc_num { get; set; }
        public string resident { get; set; }
        public string identification { get; set; }
        public string senderId { get; set; }
        public string messageId { get; set; }
        public int objectId { get; set; }
        public int senderPointID { get; set; }
        public string messageInitID { get; set; }
        public string statusName { get; set; }
        public string returnCode { get; set; }
        public string returnText { get; set; }

        public Subject()
        {}

        public Subject(string str) 
        {

                string[] arrstr = str.Split(';');
                Console.WriteLine(arrstr.Length);
                this.id = int.Parse(arrstr[0]);
                this.email = arrstr[1];
                this.last_name = arrstr[3];
                this.name = arrstr[4];

                this.second_name = arrstr[5];
                this.p_serie = arrstr[6];
                this.p_number = arrstr[7];
                this.personal_birthday = arrstr[8];
                this.p_date = arrstr[9];
                this.p_who = arrstr[10];
                this.personal_zip = arrstr[11];
                this.doc_type = arrstr[12];
                this.doc_num = arrstr[13];
                this.resident = arrstr[14];
                this.identification = arrstr[15];
                if (arrstr.Length > 16)
                {
                    this.senderId = arrstr[16];
                    this.messageId = arrstr[17];
                    this.objectId = int.Parse(arrstr[18]);
                    this.senderPointID = int.Parse(arrstr[19]);
                    this.messageInitID = arrstr[20];
                    this.statusName = arrstr[21];
                    this.returnCode = arrstr[22];
                    this.returnText = arrstr[23];
                }
        }

    }

}
