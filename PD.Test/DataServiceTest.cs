using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PD.Data;
using PD.Models;
using PD.Services;
using PD.Services.Projections;
using PD.Test.Db;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace PD.Test
{
    public class DataServiceTest : PdTestBase
    {
        public DataServiceTest()
        {

        }

        [Fact]
        public void DataProtectionTest()
        {
            var dp1 = _serviceProvider.GetService<IPdDataProtector>();
            var input = "Hello World";
            var enc = dp1.Encrypt(input);

            var dp2 = _serviceProvider.GetService<IPdDataProtector>();
            var dec = dp2.Decrypt(enc);

            Assert.Equal(input, dec);
        }

        [Fact]
        public void PersonSaveTest()
        {
            Random rand = new Random();
            var dp1 = _serviceProvider.GetService<IPdDataProtector>();
            var db = _serviceProvider.GetService<ApplicationDbContext>();

            var name = string.Format("Name {0}", rand.Next());
            var emplId = DateTime.Now.Ticks.ToString();
            Person person = new Person() { Name = dp1.Encrypt(name), EmployeeId = emplId };
            db.Persons.Add(person);
            db.SaveChanges();

            var dp2 = _serviceProvider.GetService<IPdDataProtector>();
            var decName = dp2.Decrypt(db.Persons.Where(p => p.EmployeeId == emplId).Select(p => p.Name).FirstOrDefault());

            Assert.Equal(name, decName);

        }

        [Fact]
        public void CreateSalaryScales()
        {
            ImportService ds = _serviceProvider.GetService<ImportService>();

            //Salary Scales
            //=============
            DateTime start, end;

            start = new DateTime(2014, 07, 01);
            end = new DateTime(2015, 06, 30);
            ds.AddFacultySalaryScale("AssistantProfessor", start, end, 75403, 104827, 2452, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("AssociateProfessor", start, end, 87656, 131672, 3144, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("Professor1", start, end, 109079, 131260, 3697, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("Professor2", start, end, 131261, 143836, 3144, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("Professor3", start, end, 143837, 0, 2452, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("FSO1", start, end, 63739, 88699, 2080, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("FSO2", start, end, 70500, 104828, 2452, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("FSO3", start, end, 87656, 131672, 3144, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("FSO41", start, end, 109079, 131260, 3697, 1m, 1.65m, false);
            ds.AddFacultySalaryScale("FSO42", start, end, 131261, 143837, 3144, 1m, 1.65m, false);

            start = new DateTime(2015, 07, 01);
            end = new DateTime(2016, 06, 30);
            ds.AddFacultySalaryScale("AssistantProfessor", start, end, 76534, 106402, 2489, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("AssociateProfessor", start, end, 88971, 133645, 3191, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor1", start, end, 110715, 133226, 3752, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor2", start, end, 133227, 145990, 3191, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor3", start, end, 145991, 0, 2489, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("FSO1", start, end, 64695, 90027, 2111, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("FSO2", start, end, 71558, 106404, 2489, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("FSO3", start, end, 88971, 133645, 3191, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("FSO41", start, end, 110715, 133226, 3752, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("FSO42", start, end, 133227, 145991, 3191, 1m, 1.5m, false);

            start = new DateTime(2016, 07, 01);
            end = new DateTime(2017, 06, 30);
            ds.AddFacultySalaryScale("AssistantProfessor", start, end, 77299, 107467, 2514, 1m, 1m, false);
            ds.AddFacultySalaryScale("AssociateProfessor", start, end, 89861, 134983, 3223, 1m, 1m, false);
            ds.AddFacultySalaryScale("Professor1", start, end, 111822, 134561, 3790, 1m, 1m, false);
            ds.AddFacultySalaryScale("Professor2", start, end, 134562, 147453, 3223, 1m, 1m, false);
            ds.AddFacultySalaryScale("Professor3", start, end, 147454, 0, 2514, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO1", start, end, 65342, 90926, 2132, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO2", start, end, 72274, 107470, 2514, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO3", start, end, 89861, 134983, 3223, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO41", start, end, 111822, 134561, 3790, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO42", start, end, 134562, 147454, 3223, 1m, 1m, false);

            start = new DateTime(2017, 07, 01);
            end = new DateTime(2018, 06, 30);
            ds.AddFacultySalaryScale("AssistantProfessor", start, end, 78458, 109082, 2552, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("AssociateProfessor", start, end, 91209, 137003, 3271, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor1", start, end, 113499, 136580, 3847, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor2", start, end, 136581, 149664, 3271, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("Professor3", start, end, 149665, 0, 2552, 1m, 1.5m, false);
            ds.AddFacultySalaryScale("FSO1", start, end, 66322, 92290, 2164, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO2", start, end, 73358, 109086, 2552, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO3", start, end, 91209, 137003, 3271, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO41", start, end, 113499, 136580, 3847, 1m, 1m, false);
            ds.AddFacultySalaryScale("FSO42", start, end, 136581, 149665, 3271, 1m, 1m, false);


            //Predicted values
            int start_y = 2017;
            int end_y = 2018;
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 79476, 110500, 2585, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 80494, 111918, 2618, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 81512, 113336, 2651, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 82530, 114754, 2684, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 83548, 116172, 2717, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 84566, 117590, 2750, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 85584, 119008, 2783, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 86602, 120426, 2816, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 87620, 121844, 2849, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssistantProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 88638, 123262, 2882, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 92393, 138780, 3313, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 93577, 140557, 3355, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 94761, 142334, 3397, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 95945, 144111, 3439, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 97129, 145888, 3481, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 98313, 147665, 3523, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 99497, 149442, 3565, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 100681, 151219, 3607, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 101865, 152996, 3649, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("AssociateProfessor", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 103049, 154773, 3691, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 114972, 138353, 3897, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 116445, 140126, 3947, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 117918, 141899, 3997, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 119391, 143672, 4047, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 120864, 145445, 4097, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 122337, 147218, 4147, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 123810, 148991, 4197, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 125283, 150764, 4247, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 126756, 152537, 4297, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 128229, 154310, 4347, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 138354, 151607, 3313, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 140127, 153550, 3355, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 141900, 155493, 3397, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 143673, 157436, 3439, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 145446, 159379, 3481, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 147219, 161322, 3523, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 148992, 163265, 3565, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 150765, 165208, 3607, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 152538, 167151, 3649, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 154311, 169094, 3691, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 151608, 0, 2585, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 153551, 0, 2618, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 155494, 0, 2651, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 157437, 0, 2684, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 159380, 0, 2717, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 161323, 0, 2750, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 163266, 0, 2783, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 165209, 0, 2816, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 167152, 0, 2849, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("Professor3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 169095, 0, 2882, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 67183, 93487, 2192, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 68044, 94684, 2220, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 68905, 95881, 2248, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 69766, 97078, 2276, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 70627, 98275, 2304, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 71488, 99472, 2332, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 72349, 100669, 2360, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 73210, 101866, 2388, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 74071, 103063, 2416, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO1", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 74932, 104260, 2444, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 74311, 110505, 2585, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 75264, 111924, 2618, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 76217, 113343, 2651, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 77170, 114762, 2684, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 78123, 116181, 2717, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 79076, 117600, 2750, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 80029, 119019, 2783, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 80982, 120438, 2816, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 81935, 121857, 2849, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO2", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 82888, 123276, 2882, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 92393, 138780, 3313, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 93577, 140557, 3355, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 94761, 142334, 3397, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 95945, 144111, 3439, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 97129, 145888, 3481, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 98313, 147665, 3523, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 99497, 149442, 3565, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 100681, 151219, 3607, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 101865, 152996, 3649, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO3", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 103049, 154773, 3691, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 114972, 138353, 3897, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 116445, 140126, 3947, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 117918, 141899, 3997, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 119391, 143672, 4047, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 120864, 145445, 4097, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 122337, 147218, 4147, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 123810, 148991, 4197, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 125283, 150764, 4247, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 126756, 152537, 4297, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO41", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 128229, 154310, 4347, 1m, 1.2m, true);

            start_y = 2017;
            end_y = 2018;
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 138354, 151608, 3313, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 140127, 153551, 3355, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 141900, 155494, 3397, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 143673, 157437, 3439, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 145446, 159380, 3481, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 147219, 161323, 3523, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 148992, 163266, 3565, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 150765, 165209, 3607, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 152538, 167152, 3649, 1m, 1.2m, true);
            ds.AddFacultySalaryScale("FSO42", new DateTime(++start_y, 07, 01), new DateTime(++end_y, 06, 30), 154311, 169095, 3691, 1m, 1.2m, true);

            ds.Db.SaveChanges();
        }

        [Fact]
        public void ImportFacultyData2015_16()
        {
            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;
            ImportService ds = _serviceProvider.GetService<ImportService>();

            string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts 2016 Faculty FSO Salary Adjustment Report with ChartString.xlsx";
            string worksheet2015_16 = "ARC Academic Salary Adj2015 16";
            Assert.True(File.Exists(dataFile));
            ds.InjestFacultySalaryAdjustmentData(dataFile, false, worksheet2015_16, new DateTime(2015, 7, 1).Date,
                        new DateTime(2016, 06, 30).Date, new DateTime(2017, 06, 30).Date);
            
            CreateSalaryScales();
            //ComputeFacultySalaries();
        }

        [Fact]
        public void ComputeFacultySalaries()
        {
            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;

            FacultyProjectionService srv = _serviceProvider.GetService<FacultyProjectionService>();
            for (int year = 2016; year < 2026; ++year)
            {
                var result = srv.ProjectFacultySalaries(new DateTime(year, 7, 1).Date);
                var errors = result.Errors.Distinct().ToList();
                var successes = result.Successes;
            }
        }

        [Fact]
        public void ImportFacultyData_2018_19()
        {
            ImportService ds = _serviceProvider.GetService<ImportService>();

            string dataFile = @"C:\Users\Kamal\Documents\Projects\Position-Database-Data\Arts_Increment Report_TemplateE_2018_19.xlsx";
            string worksheet = "Faculty";
            Assert.True(File.Exists(dataFile));
            ds.UploadFECData(dataFile,
                false,
                worksheet,
                new DateTime(2018, 07, 01), //Current year start date
                new DateTime(2019, 06, 30), //Current year end date
                4, //First data row
                281, //Last data row
                1, //Employee Id column
                'B' - 'A' + 1, //Name column
                'C' - 'A' + 1, // position Number
                'D' - 'A' + 1, //Rank
                'E' - 'A' + 1, //RCD
                'F' - 'A' + 1, //Dept
                'G' - 'A' + 1, //Status
                'H' - 'A' + 1, //Step on scale
                'I' - 'A' + 1, //Current salary
                'J' - 'A' + 1, //Market supplement
                'S' - 'A' + 1, //Merit decision column for the given FEC year
                'T' - 'A' + 1 //Merit reason column for the given FEC year
                 );

        }

        [Fact]
        public void ComputetOneEmployeeSalaries()
        {
            SqlServerDb server = new SqlServerDb();
            ApplicationDbContext db = server.Db;

            var dp = _serviceProvider.GetService<IPdDataProtector>();
            string employeeName = "Ingraham,Mary";
            var employees = db.Persons.Include(p => p.PositionAssignments).ToList();
            var pa_Id = employees
                .Where(empl => dp.Decrypt(empl.Name) == employeeName)
                .FirstOrDefault()
                .PositionAssignments.Select(pa => pa.Id)
                .FirstOrDefault();

            FacultyProjectionService srv = _serviceProvider.GetService<FacultyProjectionService>();
            srv.ComputeSalaries(pa_Id,
                new DateTime(2016, 7, 1).Date,
                new DateTime(2019, 7, 1).Date);
        }

    }
}