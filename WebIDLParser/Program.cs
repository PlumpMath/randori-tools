﻿
/*******************************************************************************************************

  Copyright (C) 2012 Sebastian Loncar, Web: http://loncar.de

  MIT License:

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
  associated documentation files (the "Software"), to deal in the Software without restriction, 
  including without limitation the rights to use, copy, modify, merge, publish, distribute,
  sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial
  portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT
  NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES
  OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
  CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*******************************************************************************************************/

namespace WebIDLParser
{

    public class Program
    {

        public static string OutputVersion = "1.0";

        //Path to a directory that contains all of the downloaded w3c documentation in XML form
        //which is used to generate the ASDoc comments inside the actionscript sources.
        public static string w3cDirectory = @"D:\w3c\";

        //Path to the existing project HTMLCoreLib project
        public static string csOutDirectory = @"C:\projects\Randori Framework\randori-libraries\HTMLCoreLib\src\randori\webkit\";

        //Path to the WebKit(--> WebCore) sources. ( http://trac.webkit.org/browser/trunk/Source/WebCore/ )
        public static string idlInDirectory = @"C:\projects\WebCore\";
        //public static string idlInDirectory = @"D:\IdlCs\test";

        //A temporary directory, where the preprocessed IDL files will be stored.
        public static string idlOutTempDirectory = @"D:\IdlCs\idl\";

        //Needs to be true. Set only to false, when you already have the preprocessed IDL files in the idlOutTempDirectory.
        public static bool runPreprocessor = false;
        //Needs to be true. Set only to false, when you already have the preprocessed xml files in the w3cDirectory\preprocess directory.
        public static bool runW3CXMLPreProcess = false;

        //Path to a c/c++ compiler, used for preprocessing the files.
        public static string preprocessorExe = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\VC\bin\amd64\cl.exe";

        public static void Main(string[] args)
        {
            setTransformations();
            Generator.start();
        }

        private static void setTransformations()
        {
            //In c#-bindinds, types beginning with "HTML" will be renamed to "Html".
            //THe output generates still the "HTML" version, for example
            //c#: el is HtmlImageElement
            //js: el instanceof HTMLImageElement

            Transformations.renameType("EventListener", "Function");
            Transformations.generateElementConstructorForType("HTML", "Element"); //This will extract "hr" from HtmlHrElement and generates document.createElement('hr')
            Transformations.generateElementConstructorForType("SVG", "Element");

            //Extracting the tagName will sometimes not get the correct tagname. Here they can specified more detailed.
            Transformations.generateElementConstructorCorrectTagName("HTMLImageElement", "img");
            Transformations.generateElementConstructorCorrectTagName("HTMLAnchorElement", "a");
            Transformations.generateElementConstructorCorrectTagName("HTMLTableCaptionElement", "caption");
            Transformations.generateElementConstructorCorrectTagName("HTMLTableCellElement", "td");
            Transformations.generateElementConstructorCorrectTagName("HTMLTableColElement", "col");
            Transformations.generateElementConstructorCorrectTagName("HTMLTableRowElement", "tr");
            Transformations.generateElementConstructorCorrectTagName("HTMLTableSectionElement", "tbody"); //TODO: It can be thead or tfoot, too!
            Transformations.generateElementConstructorCorrectTagName("HTMLDListElement", "dl");
            Transformations.generateElementConstructorCorrectTagName("HTMLOListElement", "ol");
            Transformations.generateElementConstructorCorrectTagName("HTMLUListElement", "ul");
            Transformations.generateElementConstructorCorrectTagName("HTMLDictionaryElement", "d");
            Transformations.generateElementConstructorCorrectTagName("HTMLParagraphElement", "p");
            Transformations.generateElementConstructorCorrectTagName("HTMLModElement", "tbody"); // TODO: Could be del or ins, but not mod. mod is an interface.

            //The Webkit IDL files have sometimes another return type for internal use. Here they can be corrected.
            Transformations.changeDelegateResultType("PositionCallback", "void");
            Transformations.changeDelegateResultType("PositionErrorCallback", "void");

            Transformations.renameType("Event", "DomEvent");

            var resolutionProperty = new TProperty(null) { name = "resolution", resultType = new TType() { name = "int"}, canRead = true, canWrite = false };
            Transformations.addPropertyToType("ImageData", resolutionProperty);
            var dataProperty = new TProperty(null) { name = "data", resultType = new TType() { name = "uint", genericType = new TType() { name="uint" } }, canRead = true, canWrite = false };
            Transformations.addPropertyToType("ImageData", dataProperty);

            var jsonMethod = new TMethod(null);
            jsonMethod.name = "JSON";
            jsonMethod.resultType = new TType() { name = "Object" };
            jsonMethod.parameters.Add(new TParameter() { name = "JSONString", type = new TType() { name = "String" } });
            Transformations.addMethodToType("Window", jsonMethod);

            var sendMethod = new TMethod(null);
            sendMethod.name = "send";
            sendMethod.resultType = new TType() { name = "void" };
            var Param = new TParameter() { name = "data", type = new TType() { name = "*" } };
            Param.attributes.Add(new TNameAttribute(){ name="Optional" });
            sendMethod.parameters.Add(Param);
            Transformations.addMethodToType("XMLHttpRequest", sendMethod);
            
            sendMethod = new TMethod(null);
            sendMethod.name = "sendArrayBuffer";
            sendMethod.aliasName = "send";
            sendMethod.resultType = new TType() { name = "void" };
            Param = new TParameter() { name = "data", type = new TType() { name = "ArrayBuffer" } };
            sendMethod.parameters.Add(Param);
            Transformations.addMethodToType("XMLHttpRequest", sendMethod);

            sendMethod = new TMethod(null);
            sendMethod.name = "sendBlob";
            sendMethod.aliasName = "send";
            sendMethod.resultType = new TType() { name = "void" };
            Param = new TParameter() { name = "data", type = new TType() { name = "Blob" } };
            sendMethod.parameters.Add(Param);
            Transformations.addMethodToType("XMLHttpRequest", sendMethod);

            sendMethod = new TMethod(null);
            sendMethod.name = "sendDocument";
            sendMethod.aliasName = "send";
            sendMethod.resultType = new TType() { name = "void" };
            Param = new TParameter() { name = "data", type = new TType() { name = "Document" } };
            sendMethod.parameters.Add(Param);
            Transformations.addMethodToType("XMLHttpRequest", sendMethod);

            sendMethod = new TMethod(null);
            sendMethod.name = "sendString";
            sendMethod.aliasName = "send";
            sendMethod.resultType = new TType() { name = "void" };
            Param = new TParameter() { name = "data", type = new TType() { name = "String" } };
            sendMethod.parameters.Add(Param);
            Transformations.addMethodToType("XMLHttpRequest", sendMethod);

            sendMethod = new TMethod(null);
            sendMethod.name = "sendFormData";
            sendMethod.aliasName = "send";
            sendMethod.resultType = new TType() { name = "void" };
            Param = new TParameter() { name = "data", type = new TType() { name = "FormData" } };
            sendMethod.parameters.Add(Param);
            Transformations.addMethodToType("XMLHttpRequest", sendMethod);
        }

    }

}

