//import java.util.regex.Pattern;

import java.io.BufferedWriter;
import java.io.FileWriter;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class Main {

    public static String[] findUnpropriateEscapeSequence(String source, String[] coloredText) {
        char[] sym = {'n','r','b','f','t'};
        for (int i = 0; i < coloredText.length - 1; ++i) {
            if (source.charAt(i) =='\\') {

            }
        }
        return coloredText;
    }

    private static boolean writeToFile(String content, String fileName) {
        try {
            BufferedWriter writer = new BufferedWriter(new FileWriter(fileName));
            writer.write(content);
            writer.close();
            return true;
        } catch (Exception e) {
            e.printStackTrace();
            return false;
        }
    }

    private static String getFileContent(String fileName) {
        try {
            return new String(Files.readAllBytes(Paths.get(fileName)));
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    public static PatternWithColor[] getPatterns() {
        return new PatternWithColor[] {
                new PatternWithColor("/[*].*[*]/", "gray"), //comment
                new PatternWithColor("//.*[\n$]?", "gray"), //comment
                new PatternWithColor("\"(?:\\\\\"|[^\"])*?\"", "DarkBlue"),  //string
                new PatternWithColor("'[^']+'", "DarkBlue"),                 // character
                new PatternWithColor("[\"][a-zA-Z \\ \" \t[:]]*[\"]", "red"),
                new PatternWithColor("#[ ]*(define|import|include|elif|else|ifndef|" +
                        "error|if|ifdef|pragma|line|undef|using|endif)", "MediumVioletRed"), //directives
                new PatternWithColor("0x[0-9A-Fa-f]*", "MediumOrchid"),
                new PatternWithColor("[-+ ]?[0-9]*[.]?[0-9]+([eE][-+]?[0-9]+)?","MediumOrchid"),
                new PatternWithColor("0[1-7][0-7]*", "MediumOrchid"),
                new PatternWithColor(
                "asm|auto|bool|break|case|catch|char|class|const|" +
                        "const_cast|continue|delete|do|dynamic_cast|far|near|new|sizeof|volatile|" +
                        "double|goto|static|explicit|export|false|" +
                        "else|huge|try|true|this|throw|typeid|switch|struct|sizeof|" +
                        "enum|if|register|typedef|operator|mutable|namespace|template|" +
                        "extern|int|inline|return|union|using|virtual|wchar_t|while"+
                        "default|for|long|unsigned|signed|void|int", "LightSeaGreen"),
                new PatternWithColor("[\\,\\;]", "Black"),
                new PatternWithColor("[\\(\\)\\+\\-\\*\\/\\.\\%\\{\\}]", "DeepPink"),
                new PatternWithColor("[_A-Za-z][0-9A-Za-z_]*", "Orange"),
                new PatternWithColor("[\\<<\\>>\\=]", "Olive"), //operators
                new PatternWithColor("[.]*", "SpringGreen"), //operators
        };
    }

    public static String[] colorText(String source) {
        String[] coloredText = new String[source.length()];
        PatternWithColor[] patterns = getPatterns();
        int k = 0;
        for (PatternWithColor pattern : patterns) {
            Matcher m = pattern.getPattern().matcher(source);
            while (m.find()) {
                boolean canColoring = true;
                for (int i = m.start(); i < m.end(); i++) {
                    if (coloredText[i] != null)
                        canColoring = false;
                }
                if (canColoring) {
                    for (int i = m.start(); i < m.end(); i++) {
                        coloredText[i] = pattern.getColor();
                    }
                }

                if (k == 2 || k == 3) {
                    for (int i = m.start(); i < m.end()-1; i++) {
                        if (source.charAt(i)=='\\') {
                            if (source.charAt(i+1) != 'n' && source.charAt(i+1) != 't' && source.charAt(i+1) != 'a'
                                    && source.charAt(i+1) != 'b' && source.charAt(i+1) != 'f' && source.charAt(i+1) != '"'
                                    && source.charAt(i+1) != '?' && source.charAt(i+1) != '\\')
                            {
                                for (int j = m.start(); j < m.end(); j++) {
                                    coloredText[j] = "SpringGreen";
                                }
                            }
                        }
                    }

                }
            }

            k++;
        }
        for (int i = 0; i < source.length(); ++i) {
            if (coloredText[i] == null)
                coloredText[i] = "SpringGreen";
        }
        return coloredText;
    }

    public static void createHtml(String source) {
        StringBuilder builder = new StringBuilder();
        String[] colors = colorText(source);
        builder.append(
                "<!DOCTYPE html>\n" +
                "<html lang=\"en\">\n" +
                "<head>\n" +
                "    <meta charset=\"UTF-8\">\n" +
                "    <title>Title</title>\n" +
                "</head>\n" +
                "<style>\n" +
                "\n" +
                "span {\n" +
                "    white-space: pre;\n" +
                "}\n" +
                "</style>\n" +
                "<body>\n" +
                "\n"
        );
        for (int i = 0; i < source.length(); i++) {
            builder.append(String.format("<span style='color:%s;'>%c</span>", colors[i], source.charAt(i)));
        }
        builder.append(
                "</body>\n" +
                "</html>"
        );
        writeToFile(builder.toString(),"index.html");
    }

    public static void main(String[] args) {
        String source = getFileContent("source.cpp");

        createHtml(source);
        //        String[] colorText = colorText(source);

    }
}
