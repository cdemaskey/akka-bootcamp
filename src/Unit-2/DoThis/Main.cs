﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Akka.Actor;
using Akka.Util.Internal;
using ChartApp.Actors;

namespace ChartApp
{
    public partial class Main : Form
    {
        private IActorRef _chartActor;
        private readonly AtomicCounter _seriesCounter = new AtomicCounter(1);
        private IActorRef _coordinatorActor;
        private IDictionary<CounterType, IActorRef> _toggleActors = new Dictionary<CounterType, IActorRef>();

        public Main()
        {
            InitializeComponent();
        }

        #region Initialization


        private void Main_Load(object sender, EventArgs e)
        {
            _chartActor = Program.ChartActors.ActorOf(Props.Create(() => new ChartingActor(sysChart)), "charting");
            var series = ChartDataHelper.RandomSeries("FakeSeries" + _seriesCounter.GetAndIncrement());
            _chartActor.Tell(new ChartingActor.InitializeChart(null));

            _coordinatorActor = Program.ChartActors.ActorOf(Props.Create(() => new PerformanceCounterCoordinatorActor(_chartActor)), "counters");

            _toggleActors[CounterType.Cpu] = Program.ChartActors.ActorOf(Props.Create(() => new ButtonToggleActor(_coordinatorActor, btnCpu, CounterType.Cpu, false))
                .WithDispatcher("akka.actor.synchronized-dispatcher"));

            _toggleActors[CounterType.Memory] = Program.ChartActors.ActorOf(Props.Create(() => new ButtonToggleActor(_coordinatorActor, btnMemory, CounterType.Memory, false))
                .WithDispatcher("akka.actor.synchonized-dispatcher"));

            _toggleActors[CounterType.Disk] = Program.ChartActors.ActorOf(Props.Create(() => new ButtonToggleActor(_coordinatorActor, btnDisk, CounterType.Disk, false))
                .WithDispatcher("akka.actor.synchronized-dispatcher"));

            _toggleActors[CounterType.Cpu].Tell(new ButtonToggleActor.Toggle());
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //shut down the charting actor
            _chartActor.Tell(PoisonPill.Instance);

            //shut down the ActorSystem
            Program.ChartActors.Terminate();
        }

        #endregion

        private void btnCpu_Click(object sender, EventArgs e)
        {

        }

        private void btnMemory_Click(object sender, EventArgs e)
        {

        }

        private void btnDisk_Click(object sender, EventArgs e)
        {

        }
    }
}
