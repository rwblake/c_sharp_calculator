namespace calculator {
	
	class Error {

        string name;
        string arg;

        public Error(string name, string arg) {
            this.name = name;
            this.arg = arg;
        }

        public string Repr() {
            return $"{this.name}, \'{this.arg}\'";
        }
    }
}
