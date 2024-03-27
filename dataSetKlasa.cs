using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomski
{
    public class dataSetKlasa
    {
        public probaDataSet dataSet;
        public probaDataSetTableAdapters.KorisniciTableAdapter korisniciTableAdapter;
        public probaDataSetTableAdapters.ProizvodTableAdapter proizvodTableAdapter;
        public probaDataSetTableAdapters.NarudzbaTableAdapter narudzbaTableAdapter;
        public probaDataSetTableAdapters.RačunTableAdapter racunTableAdapter;
        public probaDataSetTableAdapters.StavkaNarudzbeTableAdapter stavkaNarudzbeTableAdapter;
        public probaDataSetTableAdapters.StavkaRacunaTableAdapter stavkaRacunaTableAdapter;
        public static string connectionString = "Data Source=desktop-050pm3m;Initial Catalog=proba;Integrated Security=True";
        public dataSetKlasa() {
            dataSet = new probaDataSet();
            korisniciTableAdapter = new probaDataSetTableAdapters.KorisniciTableAdapter();
            proizvodTableAdapter = new probaDataSetTableAdapters.ProizvodTableAdapter();
            narudzbaTableAdapter = new probaDataSetTableAdapters.NarudzbaTableAdapter();
            racunTableAdapter = new probaDataSetTableAdapters.RačunTableAdapter();
            stavkaNarudzbeTableAdapter = new probaDataSetTableAdapters.StavkaNarudzbeTableAdapter();
            stavkaRacunaTableAdapter = new probaDataSetTableAdapters.StavkaRacunaTableAdapter();
        }

        public void popuniDataSet() {
            korisniciTableAdapter.Fill(dataSet.Korisnici);
            proizvodTableAdapter.Fill(dataSet.Proizvod);
            narudzbaTableAdapter.Fill(dataSet.Narudzba);
            racunTableAdapter.Fill(dataSet.Račun);
            stavkaNarudzbeTableAdapter.Fill(dataSet.StavkaNarudzbe);
            stavkaRacunaTableAdapter.Fill(dataSet.StavkaRacuna);
        }
    }
}
