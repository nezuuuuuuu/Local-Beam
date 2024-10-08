using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ACT4
{
    public partial class Form1 : Form
    {
        int n = 6;
        int side;
        int beamWidth = 3;
        List<SixState> beamStates;
        int moveCounter;
        int maxIterations = 1000;
        double randomnessFactor = 0.05;

        public Form1()
        {
            InitializeComponent();
            side = pictureBox1.Width / n;

            beamStates = new List<SixState>();
            for (int i = 0; i < beamWidth; i++)
            {
                beamStates.Add(randomSixState());
            }

            moveCounter = 0;
            updateUI();
        }

        private void updateUI()
        {
            pictureBox1.Refresh();
            pictureBox2.Refresh();

            var bestState = beamStates.OrderBy(s => getAttackingPairs(s)).First();
            label3.Text = "Attacking pairs: " + getAttackingPairs(bestState);
            label4.Text = "Moves: " + moveCounter;
            label5.Text = "Beam width: " + beamWidth;

            listBox1.Items.Clear();
            foreach (SixState state in beamStates)
            {
                listBox1.Items.Add(string.Join(", ", state.Y) + " (H=" + getAttackingPairs(state) + ")");
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var bestState = beamStates.OrderBy(s => getAttackingPairs(s)).First();
            drawState(e.Graphics, bestState, side);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            var bestState = beamStates.OrderBy(s => getAttackingPairs(s)).First();
            drawState(e.Graphics, bestState, side);
        }

        private void drawState(Graphics g, SixState state, int sideLength)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if ((i + j) % 2 == 0)
                    {
                        g.FillRectangle(Brushes.Black, i * sideLength, j * sideLength, sideLength, sideLength);
                    }

                    if (j == state.Y[i])
                    {
                        g.FillEllipse(Brushes.Fuchsia, i * sideLength, j * sideLength, sideLength, sideLength);
                    }
                }
            }
        }

        private SixState randomSixState()
        {
            Random r = new Random();
            SixState random = new SixState(r.Next(n), r.Next(n), r.Next(n), r.Next(n), r.Next(n), r.Next(n));
            return random;
        }

        private int getAttackingPairs(SixState f)
        {
            int attackers = 0;

            for (int rf = 0; rf < n; rf++)
            {
                for (int tar = rf + 1; tar < n; tar++)
                {
                    if (f.Y[rf] == f.Y[tar])
                        attackers++;

                    if (f.Y[tar] == f.Y[rf] + tar - rf)
                        attackers++;

                    if (f.Y[rf] == f.Y[tar] + tar - rf)
                        attackers++;
                }
            }

            return attackers;
        }

        private List<SixState> generateSuccessors(SixState state)
        {
            List<SixState> successors = new List<SixState>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (state.Y[i] != j)
                    {
                        SixState successor = new SixState(state);
                        successor.Y[i] = j;
                        successors.Add(successor);
                    }
                }
            }

            return successors;
        }

        private List<SixState> selectBestKStates(List<SixState> states, int k)
        {
            states.Sort((s1, s2) => getAttackingPairs(s1).CompareTo(getAttackingPairs(s2)));

            List<SixState> bestStates = new List<SixState>();
            for (int i = 0; i < Math.Min(k, states.Count); i++)
            {
                bestStates.Add(states[i]);
            }

            return bestStates;
        }

        private bool isGoalReached(List<SixState> states)
        {
            foreach (SixState state in states)
            {
                if (getAttackingPairs(state) == 0)
                    return true;
            }
            return false;
        }

        private void runLocalBeamSearch()
        {
            int iterations = 0;

            while (!isGoalReached(beamStates) && iterations < maxIterations)
            {
                List<SixState> nextBeam = new List<SixState>();

                foreach (SixState state in beamStates)
                {
                    List<SixState> successors = generateSuccessors(state);

                    Random r = new Random();
                    if (r.NextDouble() < randomnessFactor)
                    {
                        nextBeam.Add(successors[r.Next(successors.Count)]);
                    }
                    else
                    {
                        nextBeam.AddRange(successors);
                    }
                }

                nextBeam = selectBestKStates(nextBeam, beamWidth);

                if (beamStates.SequenceEqual(nextBeam))
                {
                    beamStates.Clear();
                    for (int i = 0; i < beamWidth; i++)
                    {
                        beamStates.Add(randomSixState());
                    }
                }
                else
                {
                    beamStates = nextBeam;
                }

                moveCounter++;
                updateUI();
                iterations++;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            runLocalBeamSearch();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            beamStates.Clear();
            for (int i = 0; i < beamWidth; i++)
            {
                beamStates.Add(randomSixState());
            }
            moveCounter = 0;
            updateUI();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            runLocalBeamSearch();
        }
    }
}
