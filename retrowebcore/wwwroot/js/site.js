// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
if ('HTMLPortalElement' in window) {
    var portal = document.createElement('portal');
    var portalClicked = false;
    document.querySelectorAll("a").forEach(function (each) {
        each.addEventListener("click", function (e) {
            e.preventDefault();
            var spinner = document.createElement('span');
            spinner.classList.add('spinner');
            spinner.classList.add('tertiary');
            spinner.clientHeight = each.clientHeight;
            //spinner.style.float = 'left';
            each.append(spinner);

            var u = new URL(e.target.href);
            u.hash = "";
            portal.addEventListener("load", e => {
                portal.activate();
            });
            portal.src = u.href;
            portal.style.display = 'none';
            document.body.appendChild(portal);

            //portal.activate();

            if (window.portalHost) {
                window.portalHost.postMessage({ done: true, frm: location.pathname });
            }
        });
    });
} else {
    document.querySelectorAll("a").forEach(function (each) {
        each.addEventListener("click", function (e) {
            e.preventDefault();
            var spinner = document.createElement('span');
            spinner.classList.add('spinner');
            spinner.classList.add('tertiary');
            spinner.clientHeight = each.clientHeight;
            //spinner.style.float = 'left';
            each.append(spinner);
            window.location = e.target.href;
        });
    });
}