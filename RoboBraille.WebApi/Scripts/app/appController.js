//controller variables
var url = 'http://2.109.50.18:5150';

var apiPaths = {
    accessibleConversion: url + '/api/AccessibleConversion',
    audio: url + '/api/Audio',
    braille: url + '/api/Braille',
    brailleMath: url + '/api/BrailleMath',
    daisy: url + '/api/Daisy',
    ebook: url + '/api/EBook',
    htmlToPdf: url + '/api/HTMLtoPDF',
    htmlToText: url + '/api/HTMLToText',
    officeConversion: url + '/api/MSOfficeConversion',
    ocr: url + '/api/OcrConversion'
};

var statusResultApiPath = url + '/api/RoboBrailleJob';

//angular code

app.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;
            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

app.controller('roboAppController', ['$scope', '$timeout', '$window', '$q', 'httpService', 'poller', function ($scope, $timeout, $window, $q, httpService, poller) {
    //init functions
    $scope.init = function () {
        $scope.audioOutputFormat = [{ id: 1, name: 'mp3' }, { id: 2, name: 'wav' }, { id: 4, name: 'wma' }, { id: 8, name: 'aac' }];
        var inputPromise = httpService.InitInputParameters(url);
        inputPromise.then(function (data) {
            //audio
            $scope.langs = data.langList;
            $scope.audio = {
                lang: data.langList[0],
                speed: 0,
                output: $scope.audioOutputFormat[0],
                gender: 2,
                age: 4,
                dkFemale: 0
            }
            $scope.brailleLangs = data.brlLangList;
            $scope.daisyLangs = data.daisyLangList;
            $scope.brlmath = data.brlMathList;
            //acc
            $scope.accformats = data.acc;
            $scope.accformatsBackup = data.acc;
            $scope.acc = data.acc[0];

            //ebook
            $scope.ebookformats = [{ id: 2, name: 'Epub3 WMO' }, { id: 2, name: 'Epub' }, { id: 1, name: 'Mobi' }, { id: 3, name: 'Epub to Txt' }, { id: 4, name: 'Epub to Rtf' }];
            $scope.ebook = $scope.ebookformats[0];

            //braille
            $scope.brlcontractionList = data.brlcont;
            $scope.brlout = [{ id: 0, name: 'None' }, { id: 1, name: 'OctoBraille' }, {id:2, name:'Unicode'}, { id: 3, name: 'Pef' }, { id: 4, name: 'NACB' }];
            $scope.brlpath = [{ id: 0, name: 'Text to Braille' }, { id: 1, name: 'Braille to Text' }];
            $scope.brlformat = [{id:6, name: '6 Dot'}, {id:8, name: '8 Dot'}];
            $scope.braille = {
                lang: data.brlLangList[0],
                contraction: data.brlcont[0],
                option: $scope.brlformat[1],
                charSet: $scope.brlout[2],
                convPath: $scope.brlpath[0],
                linesPerPage: 0,
                charsPerLine: 0,
                mathCode: $scope.brlmath[0]
            }
            //daisy
            $scope.daisy = {
                lang: data.daisyLangList[0]
            }
        }, function (reason) {
            $scope.provideErrorMessage('init:Failed: ' + reason);
        }, function (update) {
            $scope.provideErrorMessage('init:Got notification: ' + update);
        });
    };

    var initPlugin = function () {
        $('#file-upload').fileinput({
            uploadUrl: '/file-upload-single/1',
            uploadExtraData: { kvId: '10' },
            dropZoneEnabled: false,
            minFileCount: 1,
            maxFileCount: 1,
            //TODO check if to keep this method or go to the todo below
            allowedFileExtensions: ['docx', 'doc', 'pdf', 'doc', 'txt', 'png', 'gif', 'jpg', 'jpeg', 'html', 'bmp', 'tiff', 'rtf', 'xls', 'xlsx', 'csv', 'ppt', 'pptx','epub']
        }).off('filepreupload').on('filepreupload', function (event, data, previewId, index) {
            $scope.inputStepClick(0);
            if ($scope.fileTypeError) {
                return {
                    message: 'The uploaded file cannot be converted!',
                    data: { key1: '', detail1: '' }
                };
            }
        });
        //$('#file-upload').on('filebrowse', function (event) {
        //    //when you click browse
        //});
        $('#file-upload').on('fileloaded', function (event, file, previewId, index, reader) {
            //when file is loaded from computer
            $scope.myFile = file;

            if ($scope.myFile.name.match("/epub$/")) {
                $scope.myFile.type = 'application/epub+zip'
            }
        });
        //$('#file-upload').keyup(function (e) {
        //    var keyID = e.which;
        //    if (keyID == 32 || keyID == 13) {
        //        alert('key id:' + keyID);
        //    }
        //});
        $('#file-upload').on('filecustomerror', function (event, params, msg) {
            // get message
            $scope.fileTypeError = false;
            $('#file-upload').fileinput('reset').fileinput('clear').fileinput('refresh').fileinput('enable').fileinput('cancel').focus();
            alert(msg);
            //TODO alert on not allowedFileExtensions
        });
    };

    // initialize file upload plugin
    initPlugin();

    $scope.showMath = function () {
        var showM = $scope.braille.showMath;
        alert(showM);
    }

    //UI interactions
    $scope.updateOptions = function () {
        $scope.isLithuanian = false;
        $scope.hasGender = false;
        $scope.dkFemales = false;
        //hasGender: daDK (Female: Anne/Sara), nlNL,huHU,isIS,ltLT
        switch ($scope.audio.lang.code) {
            case 'ltLT':
                $scope.hasGender = true;
                $scope.isLithuanian = true;
                $scope.dkFemales = false;
                break;
            case 'daDK':
                $scope.hasGender = true;
                if ($scope.audio.gender == 2) {
                    $scope.dkFemales = true;
                }
                break;
            case 'nlNL':
            case 'huHU':
            case 'isIS':
            case 'ltLT':
                $scope.hasGender = true;
                $scope.dkFemales = false;
                break;
            default:
                $scope.isLithuanian = false;
                $scope.hasGender = false;
                $scope.dkFemales = false;
                break;

        }
    }

    $scope.danishFemales = function () {
        if ($scope.audio.lang.code == 'daDK' && $scope.audio.gender == 2) {
            $scope.isLithuanian = false;
            $scope.dkFemales = true;
        }
        else
            $scope.dkFemales = false;
    }

    //function for selecting the first step
    $scope.inputSelection = function (disc) {
        $scope.showFeedback = false;
        $scope.showFileToAudio = false;
        $scope.showAccConv = false;
        $scope.showEBook = false;
        $scope.showBraille = false;
        $scope.showDaisy = false;
        $scope.showOutputFormat = false;
        switch (disc) {
            case 0: //file
                $scope.showFileUpload = true; $scope.showUrlUpload = false; $scope.showTextUpload = false;
                $timeout(function () {
                    var $el = $('#file-upload');
                    $el[0].focus();
                }, 100);
                break;
            case 1: //url
                $scope.showFileUpload = false; $scope.showUrlUpload = true; $scope.showTextUpload = false;
                $timeout(function () {
                    $('#url-upload').focus();
                }, 100);
                break;
            case 2: //text
                $scope.showFileUpload = false; $scope.showUrlUpload = false; $scope.showTextUpload = true;
                $timeout(function () {
                    $('#text-upload').focus();
                }, 100);
                break;
            default:
                $scope.provideErrorMessage('inputSelection: switch should not be in default');
                break;
        }
    }

    $scope.allowedOutput = function (isFile) {
        $scope.accformats = $scope.accformatsBackup;
        if (isFile) {
            switch ($scope.myFile.type) {
                case 'application/vnd.openxmlformats-officedocument.wordprocessingml.document': //docx
                case 'application/msword':
                    $scope.showOptionAudio = true;
                    $scope.showOptionBraille = true;  
                    $scope.showOptionDaisy = true;
                    $scope.showOptionEbook = true;
                    $scope.showOptionAC = true; //for docx txt, PDF-A
                    var newArr = [];
                    for (var a in $scope.accformats) {
                        switch ($scope.accformats[a].value) {
                            case 'OFF_PDFA':
                            case 'OFF_Text':
                            case 'OFF_HTML':
                            case 'OFF_DOCX':
                            case 'OFF_MSWord':
                                newArr.push($scope.accformats[a]);
                                break;
                            default:
                                break;
                        }
                    }
                    $scope.accformats = newArr;
                    break;
                case 'text/plain':
                    $scope.showOptionAudio = true;
                    $scope.showOptionBraille = true;
                    $scope.showOptionDaisy = false;
                    $scope.showOptionEbook = true;
                    $scope.showOptionAC = false;
                    break;
                case 'text/html':
                    $scope.showOptionAudio = true;
                    $scope.showOptionBraille = true;
                    $scope.showOptionDaisy = false;
                    $scope.showOptionEbook = true;
                    $scope.showOptionAC = false;
                    break;
                case 'application/pdf': 
                case 'image/png':
                case 'image/bmp':
                case 'image/vnd.xiff':
                case 'image/gif':
                case 'image/jpeg':
                case 'image/x-portable-bitmap':
                case 'image/tiff':
                    $scope.showOptionAudio = true;
                    $scope.showOptionBraille = true;
                    $scope.showOptionDaisy = false;
                    $scope.showOptionEbook = true;
                    $scope.showOptionAC = true; 
                    var newArr = []; // doc docx rtf pdf-A xls xlsx csv txt html
                    for (var a in $scope.accformats) {
                        switch ($scope.accformats[a].value) {
                            case 'OFF_MSWord':
                            case 'OFF_RTF':
                            case 'OFF_PDFA':
                            case 'OFF_XLSX':
                            case 'OFF_DOCX':
                            case 'OFF_MSExcel':
                            case 'OFF_CSV':
                            case 'OFF_HTML':
                            case 'OFF_Text':
                            case 'OFF_XML':
                            case 'OFF_InternalFormat':
                                newArr.push($scope.accformats[a]);
                                break;
                            default:
                                break;
                        }
                    }
                    $scope.accformats = newArr;
                    break;
                case 'application/vnd.openxmlformats-officedocument.presentationml.presentation':
                case 'application/vnd.ms-powerpoint':
                    $scope.showOptionAudio = false;
                    $scope.showOptionBraille = false;
                    $scope.showOptionDaisy = false;
                    $scope.showOptionEbook = false;
                    $scope.showOptionAC = true; // PDF-A, html, txt, rtf
                    var newArr = []; 
                    for (var a in $scope.accformats) {
                        switch ($scope.accformats[a].value) {
                            case 'OFF_RTF':
                            case 'OFF_PDFA':
                            case 'OFF_HTML':
                            case 'OFF_Text':
                                newArr.push($scope.accformats[a]);
                                break;
                            default:
                                break;
                        }
                    }
                    $scope.accformats = newArr;
                    break;
                case 'application/rtf':
                    $scope.showOptionAudio = true;
                    $scope.showOptionBraille = true;
                    $scope.showOptionDaisy = true;
                    //show only bg-BG, da-DK, en-US, de-DE, hu-HU, pl-PL, ro-RO
                    //if contains math: NO
                    $scope.showOptionEbook = true;
                    $scope.showOptionAC = true; //txt   
                    var newArr = [];
                    for (var a in $scope.accformats) {
                        switch ($scope.accformats[a].value) {
                            case 'OFF_Text':
                                newArr.push($scope.accformats[a]);
                                break;
                            default:
                                break;
                        }
                    }
                    $scope.accformats = newArr;
                    break;
                case 'application/epub+zip':
                    $scope.showOptionAudio = false;
                    $scope.showOptionBraille = false;
                    $scope.showOptionDaisy = false;
                    $scope.showOptionEbook = true;
                    $scope.showOptionAC = false;
                    break;
                default:
                    $scope.showOptionAudio = false;
                    $scope.showOptionBraille = false;
                    $scope.showOptionDaisy = false;
                    $scope.showOptionEbook = false;
                    $scope.showOptionAC = false;
                    //display error message and put focus on it
                    $scope.fileTypeError = true;
                    break;
            }
        }
        else {
            //for url and text
            $scope.showOptionAudio = true;
            $scope.showOptionBraille = true;
            $scope.showOptionDaisy = false;
            $scope.showOptionEbook = true;
            $scope.showOptionAC = false;
        }
    }

    //decision function for the intermediate and output steps
    $scope.inputStepClick = function (disc) {
        $scope.showFeedback = false;
        $scope.showFileToAudio = false;
        $scope.showAccConv = false;
        $scope.showEBook = false;
        $scope.showBraille = false;
        $scope.showDaisy = false;
        $scope.showOutputFormat = true;
        switch (disc) {
            case 0: //file
                $scope.allowedOutput(true)
                break;
            case 1: //url extract url data
                $scope.allowedOutput(false)
                var html
                httpService.getUrlContent($scope.upload.url).then(function (res) {
                    html = res;
                });
                var blob = new Blob([html], { type: 'text/html' });
                $scope.myFile = (window.URL || window.webkitURL).createObjectURL(blob);
                break;
            case 2: //text extract text from text area
                //httpService.getTextFile($scope.upload.text).then(function (text) {
                //    $scope.myFile = text;
                //});
                $scope.allowedOutput(false)
                var blob = new Blob([$scope.upload.text], { type: 'text/plain' });
                $scope.myFile = (window.URL || window.webkitURL).createObjectURL(blob);
                break;
            case 3: //audio
                $scope.showFileToAudio = true;
                $scope.showAccConv = false;
                $scope.showEBook = false;
                $scope.showBraille = false;
                $scope.showDaisy = false;
                $timeout(function () {
                    $('#audio-languages').focus();
                }, 100);
                break;
            case 4: //braille
                $scope.showFileToAudio = false;
                $scope.showAccConv = false;
                $scope.showEBook = false;
                $scope.showBraille = true;
                $scope.showDaisy = false;
                $timeout(function () {
                    $('#braille-lang').focus();
                }, 100);
                break;
            case 5: //e-book
                $scope.showFileToAudio = false;
                $scope.showAccConv = false;
                $scope.showEBook = true;
                $scope.showBraille = false;
                $scope.showDaisy = false;
                //base font size ?
                $timeout(function () {
                    $('#ebook-formats').focus();
                }, 100);
                break;
                break;
            case 6: //accessibility conversion
                $scope.showFileToAudio = false;
                $scope.showEBook = false;
                $scope.showAccConv = true;
                $scope.showBraille = false;
                $scope.showDaisy = false;
                $timeout(function () {
                    $('#acc-formats').focus();
                }, 100);
                break;
            case 7: //daisy
                $scope.showFileToAudio = false;
                $scope.showAccConv = false;
                $scope.showEBook = false;
                $scope.showBraille = false;
                $scope.showDaisy = true;
                $timeout(function () {
                    $('#daisy-lang').focus();
                }, 100);
                break;
            default:
                break;
        }
    };

    //functions for scenarios that do not follow the main use case flow
    $scope.getTheJobStatus = function () {
        httpService.JobStatus(statusResultApiPath + '/GetJobStatus', $scope.guid).then(function (status) {
            $scope.stats = status;
            if (status == 1)
                provideOutputToUser('');
            else $scope.provideErrorMessage('getTheJobStatus: status is '+status);
        });
    }

    $scope.provideErrorMessage = function (message) {
        $scope.stats = -1;
        $scope.errormessage = message;
    }

    //action steps region: User interactions that are part of the use cases and work towards achieving the conversion
    $scope.brailleConvert = function () {
        if ($scope.myFile.type == 'text/plain') {
            $scope.dispatchJob('braillefile');
        } else if ($scope.myFile.type == 'text/html') {
            $scope.dispatchJob('braillehtml');
        } else if ($scope.myFile.type == 'application/pdf' || $scope.myFile.type == 'image/png' || $scope.myFile.type == 'image/bmp' || $scope.myFile.type == 'image/vnd.xiff'
            || $scope.myFile.type == 'image/gif' || $scope.myFile.type == '	image/jpeg' || $scope.myFile.type == 'image/x-portable-bitmap' || $scope.myFile.type == 'image/tiff') {
            $scope.dispatchJob('brailleimage');
        } else if ($scope.myFile.type == 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' || $scope.myFile.type == 'application/msword' ||
            $scope.myFile.type == 'application/vnd.openxmlformats-officedocument.presentationml.presentation' || $scope.myFile.type == 'application/vnd.ms-powerpoint' || $scope.myFile.type == 'application/rtf') {
            $scope.dispatchJob('brailleoffice');
        } else $scope.provideErrorMessage('brailleConvert: braille conversion not possible with selected file');
    };

    $scope.daisyConvert = function () {
        if ($scope.myFile.type == 'application/pdf' || $scope.myFile.type == 'image/png' || $scope.myFile.type == 'image/bmp' || $scope.myFile.type == 'image/vnd.xiff'
            || $scope.myFile.type == 'image/gif' || $scope.myFile.type == '	image/jpeg' || $scope.myFile.type == 'image/x-portable-bitmap' || $scope.myFile.type == 'image/tiff') {
            $scope.dispatchJob('daisyacc');
        }
        else {
            $scope.dispatchJob('daisyfile');
        }
    };

    $scope.convertImageToAudio = function () {
        $scope.guid = 'waiting for JobId';
        if ($scope.myFile.type == 'text/plain') {
            $scope.dispatchJob('audiofile');
        } else if ($scope.myFile.type == 'text/html') {
            $scope.dispatchJob('audiohtml');
        } else if ($scope.myFile.type == 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' || $scope.myFile.type == 'application/msword' ||
            $scope.myFile.type == 'application/vnd.openxmlformats-officedocument.presentationml.presentation' || $scope.myFile.type == 'application/vnd.ms-powerpoint' || $scope.myFile.type == 'application/rtf') {
            $scope.dispatchJob('audiooffice');
        }
        else {
            $scope.dispatchJob('audio');
        }
    };

    $scope.doAccessibleConversion = function () {
        //office to pdf, 
        if ($scope.myFile.type == 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' || $scope.myFile.type == 'application/msword' ||
            $scope.myFile.type == 'application/vnd.openxmlformats-officedocument.presentationml.presentation' || $scope.myFile.type == 'application/vnd.ms-powerpoint' || $scope.myFile.type == 'application/rtf') {
            $scope.dispatchJob('officeConv')
        } else if ($scope.myFile.type == 'text/html') { //html to text/pdf
            $scope.dispatchJob('htmlConv');
        }
        else {
            $scope.dispatchJob('accConv');
        }
    }

    $scope.ebookConvert = function () {
        if ($scope.ebook.name == 'Epub3 WMO') {
            if ($scope.myFile.type == 'application/vnd.openxmlformats-officedocument.wordprocessingml.document') {
                $scope.dispatchJob('e3wmofile');
            } else {
                $scope.dispatchJob('e3wmo');
            }
        }
        else {
            if ($scope.myFile.type == 'application/vnd.openxmlformats-officedocument.wordprocessingml.document' || 
                $scope.myFile.type == 'application/msword' || 
                $scope.myFile.type == 'application/vnd.openxmlformats-officedocument.presentationml.presentation' || 
                $scope.myFile.type == 'application/vnd.ms-powerpoint' || 
                $scope.myFile.type == 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' || 
                $scope.myFile.type == 'application/vnd.ms-excel' || 
                $scope.myFile.type == 'application/rtf') {
                $scope.dispatchJob('epubmobioffice');
            } else if ($scope.myFile.type == 'application/pdf' || $scope.myFile.type == 'image/png' || $scope.myFile.type == 'image/bmp' || $scope.myFile.type == 'image/vnd.xiff'
            || $scope.myFile.type == 'image/gif' || $scope.myFile.type == '	image/jpeg' || $scope.myFile.type == 'image/x-portable-bitmap' || $scope.myFile.type == 'image/tiff') {
                $scope.dispatchJob('epubmobi');
            } else {
                $scope.dispatchJob('epubmobifile');
            }
        }
    }

    //a job dispatcher to optimize the workflow of various conversions
    $scope.dispatchJob = function (resultType) {
        $scope.showFeedback = true;
        $scope.stats = 3;
        $scope.progressbarValue = 20;
        switch (resultType) {
            case 'accConv':
                AccConversion($scope.acc.value).then(function (guid) {
                    $scope.progressbarValue = 100;
                    provideOutputToUser(guid)
                }, function(reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'e3wmo':
                AccConversion('OFF_DOCX').then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Epub(guid, 'daisy');
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'e3wmofile':
                Epub(null, 'daisy');
                break;
            case 'epubmobi':
                AccConversion('OFF_PDFA').then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Epub(guid, 'ebook');
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'epubmobifile':
                Epub(null, 'ebook');
                break;
            case 'epubmobioffice':
                OfficeConv(null, 1).then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Epub(guid, 'ebook');
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'audio':
                AccConversion('OFF_Text').then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Audio(guid);
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'audiohtml':
                HtmlToText(null).then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Audio(guid);
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'audiooffice':
                OfficeConv(null, 2).then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Audio(guid);
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'audiofile':
                Audio(null);
                break;
            case 'htmltopdffile':
                HtmlToPdf(null).then(function (guid) {
                    $scope.progressbarValue = 100;
                    provideOutputToUser(guid);
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'htmltotextfile':
                HtmlToText(null).then(function (guid) {
                    $scope.progressbarValue = 100;
                    provideOutputToUser(guid);
                });
                break;
            case 'daisyfile': Daisy(null); break;
            case 'daisyacc':
                AccConversion('OFF_DOCX').then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Daisy(guid);
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'braillefile': Braille(null); break;
            case 'brailleimage':
                AccConversion('OFF_Text').then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Braille(guid);
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'brailleoffice':
                OfficeConv(null, 2).then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Braille(guid);
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'braillehtml':
                HtmlToText(null).then(function (guid) {
                    $scope.myFile = null;
                    $scope.progressbarValue = 50;
                    $scope.stats = 3;
                    Braille(guid);
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'officeConv':
                var output = -1;
                switch ($scope.acc.value) {
                    case 'OFF_PDFA': output = 1; break;
                    case 'OFF_Text': output = 2; break;
                    case 'OFF_HTML': output = 4; break;
                    default: break;
                }
                if (output == -1) {
                    $scope.provideErrorMessage("Conversion of office document not possible");
                    break;
                }
                OfficeConv(null, output).then(function (guid) {
                    $scope.progressbarValue = 100;
                    provideOutputToUser(guid)
                }, function (reason) {
                    $scope.provideErrorMessage(reason);
                });
                break;
            case 'htmlConv':
                
                break;
            //case 'officeconversionfile':
            //    OfficeConv(guid,'').then(function (guid) {
            //        provideOutputToUser(guid);
            //    });
            //    break;
            default:
                $scope.provideErrorMessage('dispatchJob: should not be in default');
                break;
        }
        $timeout(function () {
            $('#stats').focus();
        }, 100);
    }

    //the following functions may be both conversion endpoints or intermediate steps
    function OfficeConv(guid, output) {
        var deferred = $q.defer();
        if (output)
        var postParams = {
            MSOfficeOutput: output,
            lastjobid: guid,
            FileContent: $scope.myFile
        }

        var jobPromise = httpService.PostJobToUrl(apiPaths['officeConversion'], postParams);
        jobPromise.then(function (guid) {
            $scope.progressbarValue = 80;
            httpService.JobStatus(statusResultApiPath + '/GetJobStatus', guid).then(function (status) {
                $scope.stats = status;
                if (status == 1) {
                    deferred.resolve(guid);
                }
                else deferred.reject('Office conversion failed. status='+status);
            });
        });
        return deferred.promise;
    }

    function HtmlToPdf(guid) {
        var deferred = $q.defer();
        var postParams = {
            paperSize: "a1",
            lastjobid: guid,
            FileContent: $scope.myFile
        }

        var jobPromise = httpService.PostJobToUrl(apiPaths['htmlToPdf'], postParams);
        jobPromise.then(function (guid) {
            $scope.progressbarValue = 80;
            httpService.JobStatus(statusResultApiPath + '/GetJobStatus', guid).then(function (status) {
                $scope.stats = status;
                if (status == 1) {
                    deferred.resolve(guid);
                }
                else deferred.reject('HtmlToPdf failed. status='+status);
            });
        });
        return deferred.promise;
    }

    function AccConversion(value) {
        var deferred = $q.defer();
        var apiUrl = apiPaths['accessibleConversion'];
        var postParams =
            {
                TargetDocumentFormat: value,
                FileContent: $scope.myFile
            };
        var postPromise = httpService.PostJobToUrl(apiUrl, postParams);
        postPromise.then(function (guid) {
            //make sure status of job is marked as done
            httpService.JobStatus(statusResultApiPath + '/GetJobStatus', guid).then(function (status) {
                $scope.stats = status;
                if (status == 1) {
                    deferred.resolve(guid);
                }
                else deferred.reject('Accessible Conversion failed. status='+status);
            });
        });
        return deferred.promise;
    }

    function HtmlToText(guid) {
        var deferred = $q.defer();
        var postParams = {
            lastjobid: guid,
            FileContent: $scope.myFile
        }

        var jobPromise = httpService.PostJobToUrl(apiPaths['htmlToText'], postParams);
        jobPromise.then(function (guid) {
            $scope.progressbarValue = 80;
            httpService.JobStatus(statusResultApiPath + '/GetJobStatus', guid).then(function (status) {
                $scope.stats = status;
                if (status == 1) {
                    deferred.resolve(guid);
                }
                else deferred.reject('HtmlToText failed. status='+status);
            });
        });
        return deferred.promise;
    }

    //the following functions are only conversion endpoints
    function Audio(guid) {
        var voicepropriety = $scope.audio.gender;
        switch ($scope.audio.lang.code) {
            case 'ltLT':
                voicepropriety = $scope.audio.gender + ':' + $scope.audio.age;
                break;
            case 'daDK':
                voicepropriety = $scope.audio.gender + ':' + $scope.audio.dkFemale;
                break;
            default:
                break;
        }
        var postParams =
        {
            audiolanguage: $scope.audio.lang.code,
            speedoptions: $scope.audio.speed,
            formatoptions: $scope.audio.output.id,
            voicepropriety: voicepropriety,
            lastjobid: guid,
            FileContent: $scope.myFile
        };
        $scope.progressbarValue = 60;
        var audioPromise = httpService.PostJobToUrl(apiPaths['audio'], postParams);
        audioPromise.then(function (guid) {
            $scope.progressbarValue = 80;
            httpService.JobStatus(statusResultApiPath + '/GetJobStatus', guid).then(function (status) {
                $scope.stats = status;
                if (status == 1) {
                    $scope.progressbarValue = 100;
                    provideOutputToUser(guid);
                }
                else $scope.provideErrorMessage('Audio status =' + status);
            });
        });
    }

    function Epub(guid, path) {
        postParams =
        {
            format: $scope.ebook.id,
            daisyoutput: $scope.ebook.id,
            lastjobid: guid,
            FileContent: $scope.myFile
        };
        $scope.progressbarValue = 60;
        httpService.PostJobToUrl(apiPaths[path], postParams).then(function (guid) {
            httpService.JobStatus(statusResultApiPath + '/GetJobStatus', guid).then(function (status) {
                $scope.progressbarValue = 80;
                $scope.stats = status;
                if (status == 1) {
                    $scope.progressbarValue = 100;
                    provideOutputToUser(guid);
                }
                else $scope.provideErrorMessage('Epub status =' + status);
            });
        });
    }

    function Braille(guid) {
        var postParams = {
            BrailleFormat: $scope.braille.option.id,
            Contraction: $scope.braille.contraction.id,
            Language: $scope.braille.lang.code,
            OutputFormat: $scope.braille.charSet.id,
            ConversionPath: $scope.braille.convPath.id,
            LinesPerPage: $scope.braille.linesPerPage,
            CharactersPerLine: $scope.braille.charsPerLine,
            lastjobid: guid,
            FileContent: $scope.myFile
        }
        if ($scope.braille.hasMath) {
            postParams.push({ mathformat: $scope.braille.mathCode });
        }
        $scope.progressbarValue = 60;
        var jobPromise = null;
        if ($scope.braille.hasMath)
            jobPromise = httpService.PostJobToUrl(apiPaths['brailleMath'], postParams);
        else
            jobPromise = httpService.PostJobToUrl(apiPaths['braille'], postParams);

        jobPromise.then(function (guid) {
            $scope.progressbarValue = 80;
            httpService.JobStatus(statusResultApiPath + '/GetJobStatus', guid).then(function (status) {
                $scope.stats = status;
                if (status == 1) {
                    $scope.progressbarValue = 100;
                    provideOutputToUser(guid);
                }
                else $scope.provideErrorMessage('Braille status =' + status);
            });
        });
    }

    function Daisy(guid) {
        var postParams = {
            DaisyLanguage: $scope.daisy.lang.code,
            DaisyOutput: 1,
            lastjobid: guid,
            FileContent: $scope.myFile
        }

        $scope.progressbarValue = 60;
        var jobPromise = httpService.PostJobToUrl(apiPaths['daisy'], postParams);
        jobPromise.then(function (guid) {
            $scope.progressbarValue = 80;
            httpService.JobStatus(statusResultApiPath + '/GetJobStatus', guid).then(function (status) {
                $scope.stats = status;
                if (status == 1) {
                    $scope.progressbarValue = 100;
                    provideOutputToUser(guid);
                }
                else $scope.provideErrorMessage('Daisy status =' + status);
            });
        });
    }

    //final function to be called for providing output to the user
    function provideOutputToUser(guid) {
        if (guid != '')
            $scope.guid = guid;
        $scope.resLink = statusResultApiPath + '/GetJobResult?jobId=' + $scope.guid;
        $scope.isReady = true;
        $window.open($scope.resLink, 'Result', 'width=0,height=0');
    }
}]);