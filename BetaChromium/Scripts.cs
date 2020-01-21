namespace BetaChromium
{
    public class Scripts
    {
        public static string ClickOnPhonesJs => $@"setTimeout(()=>{{
                                                        function simulateClick2() {{
                                                          var evt = document.createEvent('MouseEvents');
                                                          evt.initMouseEvent('click', true, true, window,
                                                            0, 0, 0, 0, 0, false, false, false, false, 0, null);
                                                          var a = document.getElementsByClassName('contact-button atClickTracking contact-a')[0]; 
                                                          a.dispatchEvent(evt);      
                                                        }}
                                                        simulateClick2();
                                                        console.log('clickShowPhone2')
                                                    }}, 3*1000)";

        public static string GetPhonesFunc => @"(function(){
                                                    let elem = $('.overh.fleft.marginleft10>strong')[0];
                                                    console.log(`extractPhone: ${elem}`);
                                                    return elem ? elem.innerText: 'undefined'; 
                                                })()";
    }
}
