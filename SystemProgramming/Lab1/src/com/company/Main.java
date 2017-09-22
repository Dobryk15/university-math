package com.company;

import java.io.*;
import java.util.Set;
import java.util.TreeSet;

public class Main {

    static final String inputFileName = "input.txt";
    static BufferedReader reader;
    static Writer writer = null;
    static FileInputStream input;

    public static void main(String[] args) {
        if (!openFile()) {
            return;
        }
        try {
            reader = new BufferedReader(new InputStreamReader(input, "utf8"));
        } catch (UnsupportedEncodingException ex) {
            ex.printStackTrace();
            return;
        }
        Set<String> validWords = new TreeSet<String>();
        String word;
        while (!(word = newWord().toLowerCase()).isEmpty()) {
            if (word.length() == 31) continue;
            if (word.matches("[aeiouаеыиоуіеэ]*")) {
                validWords.add(word);
            }
        }
        writeInFile(validWords);
    }

    private  static  void  writeInFile(Set<String> validWords) {
        try {
            writer = new BufferedWriter(new OutputStreamWriter(
                    new FileOutputStream("output.txt"), "utf-8"));
            writer.write("Words with only vowels:" + '\n');

            for (String item : validWords) {
                writer.write(item + '\n');
            }
        } catch (IOException ex) {
            System.out.println(ex);
            return;
        } finally {
            try {writer.close();} catch (Exception ex) {/*ignore*/}
        }
    }
    private static boolean openFile() {
        try {
            input = new FileInputStream(inputFileName);
        } catch (FileNotFoundException ex) {
            ex.printStackTrace();
            return false;
        }
        return true;
    }

    private static String newWord() {
        String word = "";
        int ch;
        try {
            while ((ch = reader.read()) != -1 && !Character.isLetter(ch)); // !Character.isAlphabetic(ch));
            if (ch == -1) {
                return "";
            }
            word += (char)ch;
            while ((ch = reader.read()) != -1 && Character.isLetter(ch)) {
                if (word.length() < 31) {
                    word += (char)ch;
                }
            }
        } catch (IOException ex) {
            ex.printStackTrace();
        } finally {
            return word;
        }
    }
}
