﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using Db4objects.Db4o;

namespace MDB.GUI
{
    public partial class addMovie : Form, addWatchable
    {
        //        ArrayList allAwards = new ArrayList();
        //        public List<String> castNames = new List<String>();
        public static ArrayList wonAward;
        public static ArrayList nomAward;
        public ArrayList mainCast = new ArrayList();
        Image posterImage;

        public addMovie()
        {
            InitializeComponent();
        }

        public void addPersonToCast(int personID)
        {
            mainCast.Add(Person.GetPersonByID(personID));
            listBox1.Items.Add(Person.GetPersonByID(personID).GetName().GetFirstName() + " " + Person.GetPersonByID(personID).GetName().GetLastName());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            addPerson newPerson = new addPerson(true, this);
            newPerson.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.ImageLocation = openFileDialog1.FileName;
                posterImage = new Bitmap(openFileDialog1.FileName);
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<string> genre = checkedListBox1.CheckedItems.OfType<string>().ToList();
            List<Person> person = mainCast.Cast<Person>().ToList();
            string MPAA = comboBox1.Text;
            string synopsis = richTextBox1.Text;
            string production = comboBox2.Text;
            int rating = Convert.ToInt32(textBox2.Text);
            string title = textBox1.Text;
            DateTime released = dateTimePicker1.Value.Date;
            int time = Convert.ToInt32(textBox3.Text);

            if (!Movie.Exists(title))
            {
                Movie newMovie = new Movie(new List<Award>(), new List<Award>(), genre, person,
                MPAA, synopsis, production, rating, new List<User>(), title, posterImage, released, time);
                MessageBox.Show(@"Movie successfully added");

                //Adding 'Watchable' and 'Person' back into 'Feature'
                for (int i = 0; i < newMovie.GetMainCast().Count; i++)
                {
                    List<Feature> op = newMovie.GetMainCast()[i].GetFeatures();
                    op[op.Count - 1].SetEntity(newMovie);
                    op[op.Count - 1].SetPerson(newMovie.GetMainCast()[i]);
                }
            }
            else
            {
                Movie newMovie = Movie.GetMovieByTitle(title);
                newMovie.SetGenre(genre);
                newMovie.SetMainCast(person);
                newMovie.SetMpaaRating(MPAA);
                newMovie.SetSynopsis(synopsis);
                newMovie.SetProductionStatus(production);
                newMovie.SetRating(rating);
                newMovie.SetReleaseDate(released);
                newMovie.SetRunTime(time);
                newMovie.setPoster(posterImage);

                MessageBox.Show(@"Movie successfully edited");
                //Adding 'Watchable' and 'Person' back into 'Feature'
                for (int i = 0; i < newMovie.GetMainCast().Count; i++)
                {
                    List<Feature> op = newMovie.GetMainCast()[i].GetFeatures();
                    op[op.Count - 1].SetEntity(newMovie);
                    op[op.Count - 1].SetPerson(newMovie.GetMainCast()[i]);
                }
            }


        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Movie x = new Movie();
            string title = textBox1.Text;
            IObjectSet AllObjects = MultimediaDB.db.QueryByExample(typeof(Movie));
            if (!Movie.Exists(textBox1.Text))
            {
                MessageBox.Show(@"Movie does not exist in the database");
                ClearAll();
            }
            else
            {
                for (int i = 0; i < AllObjects.Count; i++)
                {
                    x = (Movie)AllObjects[i];
                    if (x.GetTitleName().Equals(title))
                    {
                        Lookup(x);
                    }
                }
            }
        }

        private void CheckBoxLookup(Movie movie)
        {
            for (int j = 0; j < checkedListBox1.Items.Count; j++) //clears check box
            {
                checkedListBox1.SetItemChecked(j, false);
            }

            for (int i = 0; i < movie.GetGenre().Count; i++)
            {
                for (int j = 0; j < checkedListBox1.Items.Count; j++)
                {
                    if (movie.GetGenre()[i].Equals(checkedListBox1.Items[j].ToString()))
                    {
                        checkedListBox1.SetItemChecked(j, true); //check matching genre
                    }
                }
            }
        }

        private void FillCastNames(Movie movie)
        {
            listBox1.Items.Clear();
            for (int i = 0; i < movie.GetMainCast().Count; i++)
            {
                listBox1.Items.Add(movie.GetMainCast()[i].GetName().GetFirstName() + " " +
                                   movie.GetMainCast()[i].GetName().GetLastName());
            }
        }

        private void Lookup(Movie x)
        {
            //            List<String> genre = checkedListBox1.CheckedItems.OfType<String>().ToList();
            //            List<Person> person = mainCast.Cast<Person>().ToList();
            //            String MPAA = comboBox1.Text;
            //            String synopsis = richTextBox1.Text;
            //            String production = comboBox2.Text;
            //            int rating = Convert.ToInt32(textBox2.Text);
            //            String title = textBox1.Text;
            //            DateTime released = dateTimePicker1.Value.Date;
            //            int time = Convert.ToInt32(textBox3.Text);

            CheckBoxLookup(x);
            FillCastNames(x);
            comboBox1.Text = x.GetMpaaRating();
            richTextBox1.Text = x.GetSynopsis();
            comboBox2.Text = x.GetProductionStatus();
            textBox2.Text = x.GetRating().ToString();
            textBox1.Text = x.GetTitleName();
            dateTimePicker1.Value = x.GetReleaseDate();
            textBox3.Text = x.GetRunTime().ToString();
            posterImage = pictureBox1.Image = x.getPoster();
        }

        private void ClearAll()
        {
            for (int j = 0; j < checkedListBox1.Items.Count; j++) //clears check box
            {
                checkedListBox1.SetItemChecked(j, false);
            }
            listBox1.Items.Clear();
            comboBox1.Text = "";
            richTextBox1.Text = "";
            comboBox2.Text = "";
            textBox2.Text = "";
            textBox1.Text = "";
            dateTimePicker1.Value = DateTime.Now;
            textBox3.Text = "";
        }
    }
}
