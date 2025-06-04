# sota staircase sidestepper getter
# excerpted from https://github.com/markjoshwel/Interchange/blob/main/ReStepper/restepper.py

from json import loads as json_loads

# noinspection PyProtectedMember
from os import X_OK, access
from pathlib import Path
from platform import machine, system
from pprint import pformat
from subprocess import CompletedProcess
from subprocess import run as _run
from sys import argv, stderr
from textwrap import indent
from traceback import format_tb
from typing import Final
from urllib.request import urlopen

# constants
INDENT: Final[str] = "   "
VERBOSE: Final[bool] = "--verbose" in argv

SIDESTEPPER_PRIMARY_LINK: Final[str] = (
    "https://forge.joshwel.co/api/v1/repos/mark/sidestepper/releases/latest"
)
SIDESTEPPER_SECONDARY_LINK: Final[str] = (
    "https://api.github.com/repos/markjoshwel/sidestepper/releases/latest"
)
SIDESTEPPER_GLOBAL_BINARY_PATH: Final[Path] = (
    Path()
    .home()
    .joinpath(
        ".local/bin/sidestepper.exe"
        if (system().lower() == "windows")
        else ".local/bin/sidestepper"
    )
)


# dictionary to share state across steps
r: dict[str, str] = {}


# define these before importing third-party modules because we use them in the import check
def generate_command_failure_message(cp: CompletedProcess) -> str:
    return "\n".join(
        [
            f"\n\nerror: command '{cp.args}' failed with exit code {cp.returncode}",
            f"{INDENT}stdout:",
            (
                indent(text=cp.stdout.decode(), prefix=f"{INDENT}{INDENT}")
                if (isinstance(cp.stdout, bytes) and (cp.stdout != b""))
                else f"{INDENT}{INDENT}(no output)"
            ),
            f"{INDENT}stderr:",
            (
                indent(text=cp.stderr.decode(), prefix=f"{INDENT}{INDENT}")
                if (isinstance(cp.stderr, bytes) and (cp.stderr != b""))
                else f"{INDENT}{INDENT}(no output)"
            )
            + "\n",
        ]
    )


def log_err(
    message: str | CompletedProcess,
    exception: Exception | None = None,
    condition: bool = True,
    exitcode: int | None = None,
    show_r: bool = False,
) -> None:
    if not condition:
        return

    if isinstance(message, CompletedProcess):
        print(
            generate_command_failure_message(message)
            + (
                ("\n" + indent(text=pformat(r), prefix=INDENT) + "\n") if show_r else ""
            ),
            file=stderr,
            flush=True,
        )

    elif isinstance(exception, Exception):
        print(
            f"\n\nerror: {exception}",
            f"{INDENT}{exception.__class__.__name__}: {exception}",
            indent(text=pformat(r), prefix=INDENT),
            indent(text="\n".join(format_tb(exception.__traceback__)), prefix=INDENT)
            + (
                ("\n" + indent(text=pformat(r), prefix=INDENT) + "\n") if show_r else ""
            ),
            sep="\n",
            file=stderr,
            flush=True,
        )

    else:
        print(
            f"error: {message}"
            + (
                ("\n" + indent(text=pformat(r), prefix=INDENT) + "\n") if show_r else ""
            ),
            file=stderr,
            flush=True,
        )

    if isinstance(exitcode, int):
        exit(exitcode)


def log_debug(message: str) -> None:
    if VERBOSE:
        print(f"debug: {message}", file=stderr)


def run(
    command: str | list[str],
    wd: Path | str | None = None,
    capture_output: bool = True,
    give_input: str | None = None,
) -> CompletedProcess[bytes]:
    """
    exception-safe-ish wrapper around subprocess.run()

    args:
        command: str | list
            the command to run
        cwd: Path | str | None = None
            the working directory
        capture_output: bool = True
            whether to capture the output
        give_input: str | None = None
            the input to give to the command

    returns: CompletedProcess
        the return object from subprocess.run()
    """

    # noinspection PyBroadException
    try:
        cp = _run(
            command,
            shell=False if isinstance(command, list) else True,
            cwd=wd,
            capture_output=capture_output,
            input=give_input.encode() if give_input else None,
        )
    except Exception as run_exc:
        log_err(f"command '{command}' failed with exception", run_exc)
        exit(-1)
    return cp


def _sidestepper_resolve_binary_name() -> str:
    os: str
    match system().lower():
        case "windows":
            os = "windows"
        case "linux":
            os = "linux"
        case "darwin":
            os = "macos"
        case _:
            os = "unknown"

    arch: str
    match machine().lower():
        case "x86_64":
            arch = "x86_64"
        case "amd64":
            arch = "x86_64"
        case "aarch64" | "arm64":
            arch = "aarch64"
        case _:
            arch = "unknown"

    match (os, arch):
        case ("unknown", _):
            return ""
        case (_, "unknown"):
            return ""
        case _:
            if os == "windows":
                return f"sidestepper-{os}-{arch}.exe"
            return f"sidestepper-{os}-{arch}"


def _sidestepper_resolve_binary_path(root: Path | None) -> Path | str:
    """returns path if found, empty string if not found, error message if error"""

    sidestepper_binary_name = _sidestepper_resolve_binary_name()
    if sidestepper_binary_name == "":
        return "could not determine sidestepper binary name, your platform is probably unsupported"

    if root:
        for possible_sidestepper in (
            root.joinpath(sidestepper_binary_name),
            root.joinpath(SIDESTEPPER_GLOBAL_BINARY_PATH.name),
            root.joinpath(f"Tooling/{sidestepper_binary_name}"),
            root.joinpath(f"Tooling/{SIDESTEPPER_GLOBAL_BINARY_PATH.name}"),
        ):
            log_debug(
                f"_sidestepper_resolve_binary_path: trying to use '{possible_sidestepper}'"
            )
            if not possible_sidestepper.exists():
                continue
            if not possible_sidestepper.is_file():
                return f"'{possible_sidestepper}' is not a file, this should not happen"
            if not access(possible_sidestepper, X_OK):
                return f"'{possible_sidestepper}' is not executable, this should not happen"
            return possible_sidestepper

    log_debug(
        f"_sidestepper_resolve_binary_path: trying to use '{SIDESTEPPER_GLOBAL_BINARY_PATH}'"
    )
    if not SIDESTEPPER_GLOBAL_BINARY_PATH.exists():
        return ""
    if not SIDESTEPPER_GLOBAL_BINARY_PATH.is_file():
        return f"'{SIDESTEPPER_GLOBAL_BINARY_PATH}' exists but is not a file, this should not happen"
    if not access(SIDESTEPPER_GLOBAL_BINARY_PATH, X_OK):
        return "'{SIDESTEPPER_GLOBAL_BINARY_PATH}' exists but is not executable, this should not happen"
    return SIDESTEPPER_GLOBAL_BINARY_PATH


def _sidestepper_resolve_version_file() -> Path | str:
    """
    creates parent directories if needed but not the file itself,
    returns the path if found, error message if error
    """
    match system().lower():
        case "windows":
            version_path = (
                Path()
                .home()
                .joinpath(
                    "AppData/Roaming/sota staircase/sidestepper/sidestepper.version.txt"
                )
            )
        case "linux":
            version_path = (
                Path()
                .home()
                .joinpath(".local/share/sotastaircase/sidestepper.version.txt")
            )
        case "darwin":
            version_path = (
                Path()
                .home()
                .joinpath(
                    "Library/Application Support/co.joshwel.sotastaircase/sidestepper/sidestepper.version.txt"
                )
            )
        case _:
            version_path = (
                Path()
                .home()
                .joinpath(".local/share/sotastaircase/sidestepper.version.txt")
            )

    if not version_path.exists():
        try:
            version_path.parent.mkdir(parents=True, exist_ok=True)
        except Exception as e:
            return f"could not create directory '{version_path.parent}' ({e.__class__.__name__}: {e})"

    return version_path


def _sidestepper_download_latest() -> str:
    """used to download to the global binary path, returns an error message if error"""

    # _sidestepper_resolve_binary_path() was called before this,
    # so we know the parent directories exist
    sidestepper_binary_name = _sidestepper_resolve_binary_name()
    if sidestepper_binary_name == "":
        return "could not determine sidestepper binary name, your platform is probably unsupported"
    sidestepper_version_path = _sidestepper_resolve_version_file()
    if isinstance(sidestepper_version_path, str):
        return sidestepper_version_path

    log_debug(
        f"_sidestepper_download_latest: resolved binary name to '{sidestepper_binary_name}'"
    )
    log_debug(
        f"_sidestepper_download_latest: resolved version file path to '{sidestepper_version_path}'"
    )

    version_tag: str = ""
    download_url: str = ""

    # forge (gitea) and github have basically the same api
    for name, link in (
        ("primary", SIDESTEPPER_PRIMARY_LINK),
        ("secondary", SIDESTEPPER_SECONDARY_LINK),
    ):
        log_debug(f"_sidestepper_download_latest: checking {name} api endpoint {link}")
        try:
            with urlopen(link) as response_json:
                response_json = json_loads(response_json.read().decode("utf-8"))
                version_tag = response_json["tag_name"]
                for asset in response_json["assets"]:
                    if asset["name"].lower() == sidestepper_binary_name.lower():
                        download_url = asset["browser_download_url"]
                        log_debug(
                            f"_sidestepper_download_latest: retrieval successful; using {name} api endpoint {link}"
                        )
                        break
                else:
                    continue
                break
        except Exception as e:
            print(
                f"warning: could not fetch latest sidestepper release from {name} api endpoint {link} ({e.__class__.__name__}: {e})",
                file=stderr,
            )

    if (not version_tag) or (not download_url):
        return "could not fetch latest sidestepper release from any api endpoint"

    # check if we already have the latest sidestepper release
    if sidestepper_version_path.exists():
        current_version_tag = sidestepper_version_path.read_text(
            encoding="utf-8"
        ).strip()
        if (
            (current_version_tag == version_tag)
            and (SIDESTEPPER_GLOBAL_BINARY_PATH.exists())
            and (access(SIDESTEPPER_GLOBAL_BINARY_PATH, X_OK))
        ):
            log_debug("_sidestepper_download_latest: nothing to do, returning early")
            return ""

    # download the latest sidestepper release
    log_debug(
        f"_sidestepper_download_latest: downloading latest release from {download_url}"
    )
    try:
        SIDESTEPPER_GLOBAL_BINARY_PATH.parent.mkdir(parents=True, exist_ok=True)
        with urlopen(download_url) as response:
            SIDESTEPPER_GLOBAL_BINARY_PATH.write_bytes(response.read())
    except Exception as e:
        return f"could not download latest sidestepper release {version_tag} from {download_url} to {SIDESTEPPER_GLOBAL_BINARY_PATH} ({e.__class__.__name__}: {e})"

    # make it executable (non-windows)
    if not (system().lower() == "windows"):
        try:
            SIDESTEPPER_GLOBAL_BINARY_PATH.chmod(0o755)
        except Exception as e:
            return (
                f"could not make latest sidestepper release {version_tag} located at {SIDESTEPPER_GLOBAL_BINARY_PATH} executable ({e.__class__.__name__}: {e}) "
                f"- try running `chmod +x '{SIDESTEPPER_GLOBAL_BINARY_PATH}'`"
            )

    # write the latest sidestepper version to the version file
    log_debug(
        f"_sidestepper_download_latest: writing latest sidestepper version {version_tag} to '{sidestepper_version_path}'"
    )
    try:
        sidestepper_version_path.parent.mkdir(parents=True, exist_ok=True)
        with open(sidestepper_version_path, "w") as version_file:
            version_file.write(version_tag)
    except Exception as e:
        return f"could not write latest sidestepper version {version_tag} to '{sidestepper_version_path}' ({e.__class__.__name__}: {e})"

    return ""


def sidestepper_get(root: Path | None) -> Path | str:
    """returns a path if successfully retrieved, empty string if not found, error message if error"""

    sidestepper: Path | str = _sidestepper_resolve_binary_path(root)
    log_debug(f"sidestepper_get: resolved binary path: '{sidestepper}'")

    if isinstance(sidestepper, Path):
        if root and (str(root.absolute()) in str(sidestepper.absolute())):
            # we are using a local sidestepper binary (in the repo)
            return sidestepper

        # we are using a global sidestepper binary (in ~/.local/bin)
        # let's try to update it
        log_debug("sidestepper_get: attempting update")
        dl_err = _sidestepper_download_latest()
        if dl_err:
            print(
                f"warning: tried to update sidestepper but failed, sweeping under the rug and continuing ({dl_err})",
                file=stderr,
            )

        return sidestepper

    # _sidestepper_resolve_binary_path returned an error string,
    # and it is not empty, propagate it up to the caller to print
    if isinstance(sidestepper, str) and sidestepper:
        return sidestepper

    # so let's download the latest sidestepper binary
    log_debug("sidestepper_get: downloading latest")
    dl_err = _sidestepper_download_latest()
    if dl_err:
        return dl_err
    return SIDESTEPPER_GLOBAL_BINARY_PATH


def get_root() -> Path | None:
    """finds the root of the git repository, returns None if not found"""
    root: Path = Path().cwd().resolve()
    while not (root.joinpath(".git").exists()):
        root = root.parent
        if root == Path("/").resolve():
            return None
    return root


def main() -> None:
    repo_path = get_root()
    log_err(
        "could not find a git repository in the working or parent directories",
        condition=not isinstance(repo_path, Path),
        exitcode=1,
    )
    assert isinstance(repo_path, Path)

    sidestepper_binary = sidestepper_get(repo_path)
    log_err(
        "could not find sidestepper binary",
        condition=(sidestepper_binary == ""),
        exitcode=2,
    )
    log_err(
        sidestepper_binary,  # type: ignore
        condition=isinstance(sidestepper_binary, str),
        exitcode=3,
    )
    log_debug(f"sidestepper binary is '{sidestepper_binary}'")
    assert isinstance(sidestepper_binary, Path)

    run([str(sidestepper_binary)], wd=repo_path, capture_output=False)


if __name__ == "__main__":
    main()
