namespace Plugin.Data {
    internal class ReporterMessage {
        public int duration;
        public int states;
        public int maxY;
        public int incrementY;
        public ReporterMessage() {
        }

        public ReporterMessage(int duration, int states, int maxY, int incrementY) {
            this.duration = duration;
            this.states = states;
            this.maxY = maxY;
            this.incrementY = incrementY;
        }
    }
}
