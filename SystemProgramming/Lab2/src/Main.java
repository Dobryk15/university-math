import java.util.Set;

public class Main {

    public static void main(String[] args) {
        FiniteAutomaton automaton = new FiniteAutomaton();
        if (!automaton.readFromFile("input.txt")) {
            System.out.print("Wrong input");
            return;
        }

        Set<String> states = automaton.getReachableStates();
        for (String s : states) {
            System.out.println(s);
        }

        String[] tests = {"ab", "cd"};
        for (String test: tests) {
            boolean reachable = automaton.isReachableWord(test, states);
            System.out.printf("%s: %b\n", test, reachable);
        }
    }
}
