namespace Plugin.Data {
    internal class ReporterMessage {
        public int duration = 0;
        public int states = 0;
        public int maxY = 0;
        public int incrementY = 0;
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
