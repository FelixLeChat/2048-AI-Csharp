import cProfile
from StateSearch import *
from LocalSearch import *
from AntennaVisualisation import *
import pstats
from lineProfiler import *
from random import randint



def startSearch():

    # 12 points, Cost:1471, Step:1809, Time: 24,103 sec State generate: 52953
    # Pas solution optimale: vrai solution
    """
  -  {'middle': (45, 45), 'points': set([3713024257993580481, 3713030753124095031, 3713061063873891181, 3713030753134920281]), 'radius': 16.0}
-{'middle': (40, 80), 'points': set([3713037248191823131]), 'radius': 1}
-{'middle': (75, 80), 'points': set([3712993947061920131, 3713017762821929981]), 'radius': 5.0}
-{'middle': (60, 10), 'points': set([3713024257932959081]), 'radius': 1}
-{'middle': (25, 10), 'points': set([3713061063899871781, 3713067559082347531, 3713037248289250381, 3713074054245337831]), 'radius': 15.0}
    :return:
    """
    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80)]

    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40),(60,10),(70,80),(80,80),(50,50),(40,10),(60,50),(40,80),(50,70),(30,60),(40,60),(40,40)]

    #positions = [(30,0),(10,10),(20,20),(30,40),(50,40)]

    #positions = [(246, 111), (288, 172), (159, 220), (176, 17), (111, 54), (288, 252), (290, 111), (133, 12), (271, 143), (168, 270), (6, 29), (296, 264), (124, 160), (298, 8), (24, 0), (293, 224), (236, 29), (86, 125), (268, 20), (180, 90), (219, 240), (167, 106), (222, 158), (56, 148), (198, 22), (110, 121), (196, 117), (116, 27), (257, 220), (160, 115), (128, 154), (253, 167), (211, 127), (54, 186), (225, 26), (152, 31), (211, 204), (115, 89), (187, 280), (224, 18), (280, 195), (23, 168), (62, 48), (230, 47), (175, 261), (281, 65), (184, 224), (89, 221), (294, 140), (295, 71)]

    #positions = [(10,30),(10,50),(0,50)]

    position = [( 628 , 366 ), ( 126 , 601 ), ( 892 , 691 ), ( 380 , 192 ), ( 319 , 823 ), ( 541 , 984 ), ( 680 , 861 ), ( 144 , 186 ), ( 464 , 618 ), ( 688 , 305 ), ( 882 , 967 ), ( 102 , 431 ), ( 392 , 965 ), ( 837 , 387 ), ( 331 , 146 ), ( 709 , 877 ), ( 530 , 464 ), ( 216 , 928 ), ( 112 , 623 ), ( 636 , 728 ), ( 689 , 657 ), ( 539 , 589 ), ( 574 , 412 ), ( 99 , 94 ), ( 857 , 166 ), ( 83 , 816 ), ( 138 , 282 ), ( 756 , 370 ), ( 49 , 812 ), ( 966 , 6 ), ( 533 , 815 ), ( 177 , 867 ), ( 571 , 756 ), ( 176 , 729 ), ( 269 , 741 ), ( 629 , 781 ), ( 362 , 898 ), ( 712 , 797 ), ( 268 , 205 ), ( 434 , 137 ), ( 479 , 170 ), ( 130 , 551 ), ( 158 , 271 ), ( 489 , 267 ), ( 106 , 914 ), ( 568 , 830 ), ( 919 , 211 ), ( 429 , 932 ), ( 177 , 641 ), ( 382 , 967 ), ( 668 , 60 ), ( 108 , 626 ), ( 549 , 505 ), ( 278 , 901 ), ( 928 , 185 ), ( 641 , 484 ), ( 253 , 147 ), ( 352 , 29 ), ( 864 , 174 ), ( 826 , 264 ), ( 153 , 516 ), ( 907 , 571 ), ( 568 , 433 ), ( 746 , 492 ), ( 461 , 227 ), ( 716 , 925 ), ( 800 , 721 ), ( 720 , 245 ), ( 924 , 623 ), ( 91 , 387 ), ( 794 , 203 ), ( 692 , 587 ), ( 335 , 202 ), ( 98 , 883 ), ( 858 , 955 ), ( 440 , 93 ), ( 999 , 765 ), ( 848 , 475 ), ( 796 , 347 ), ( 647 , 474 ), ( 555 , 991 ), ( 705 , 270 ), ( 304 , 1 ), ( 997 , 651 ), ( 714 , 112 ), ( 266 , 436 ), ( 257 , 791 ), ( 612 , 933 ), ( 979 , 168 ), ( 408 , 404 ), ( 989 , 654 ), ( 546 , 485 ), ( 783 , 801 ), ( 841 , 439 ), ( 383 , 916 ), ( 618 , 64 ), ( 826 , 895 ), ( 567 , 993 ), ( 585 , 739 ), ( 442 , 870 ), ( 703 , 380 ), ( 151 , 279 ), ( 312 , 764 ), ( 579 , 242 ), ( 256 , 723 ), ( 439 , 122 ), ( 371 , 189 ), ( 88 , 322 ), ( 97 , 919 ), ( 511 , 557 ), ( 886 , 312 ), ( 980 , 49 ), ( 994 , 327 ), ( 341 , 704 ), ( 960 , 757 ), ( 783 , 886 ), ( 481 , 672 ), ( 673 , 110 ), ( 193 , 103 ), ( 176 , 300 ), ( 934 , 109 ), ( 339 , 18 ), ( 787 , 190 ), ( 727 , 688 ), ( 645 , 617 ), ( 418 , 942 ), ( 9 , 751 ), ( 545 , 578 ), ( 254 , 244 ), ( 421 , 911 ), ( 280 , 516 ), ( 630 , 972 ), ( 830 , 225 ), ( 618 , 663 ), ( 526 , 127 ), ( 687 , 709 ), ( 933 , 335 ), ( 424 , 138 ), ( 154 , 276 ), ( 223 , 972 ), ( 543 , 931 ), ( 435 , 996 ), ( 190 , 166 ), ( 994 , 585 ), ( 546 , 403 ), ( 970 , 796 ), ( 813 , 915 ), ( 786 , 974 ), ( 971 , 193 ), ( 359 , 337 ), ( 686 , 342 ), ( 154 , 13 ), ( 556 , 711 ), ( 593 , 222 ), ( 110 , 742 ), ( 213 , 783 ), ( 770 , 560 ), ( 481 , 454 ), ( 875 , 819 ), ( 275 , 13 ), ( 9 , 882 ), ( 222 , 264 ), ( 693 , 789 ), ( 124 , 544 ), ( 54 , 618 ), ( 662 , 434 ), ( 36 , 819 ), ( 840 , 114 ), ( 124 , 163 ), ( 10 , 509 ), ( 890 , 244 ), ( 636 , 735 ), ( 12 , 370 ), ( 145 , 605 ), ( 392 , 270 ), ( 69 , 848 ), ( 244 , 782 ), ( 462 , 609 ), ( 600 , 314 ), ( 803 , 577 ), ( 909 , 706 ), ( 679 , 220 ), ( 836 , 509 ), ( 478 , 651 ), ( 517 , 918 ), ( 217 , 490 ), ( 949 , 910 ), ( 614 , 422 ), ( 864 , 29 ), ( 736 , 711 ), ( 360 , 294 ), ( 175 , 63 ), ( 821 , 2 ), ( 106 , 386 ), ( 500 , 972 ), ( 444 , 359 ), ( 161 , 755 ), ( 384 , 509 ), ( 235 , 292 ), ( 624 , 496 ), ( 636 , 764 ), ( 18 , 745 ), ( 328 , 909 ), ( 414 , 827 ), ( 181 , 580 ), ( 48 , 206 ), ( 301 , 169 ), ( 877 , 681 ), ( 260 , 542 ), ( 255 , 878 ), ( 620 , 39 ), ( 631 , 919 ), ( 884 , 205 ), ( 180 , 498 ), ( 935 , 823 ), ( 705 , 350 ), ( 188 , 154 ), ( 71 , 923 ), ( 173 , 957 ), ( 186 , 854 ), ( 97 , 754 ), ( 799 , 291 ), ( 520 , 308 ), ( 174 , 521 ), ( 410 , 304 ), ( 30 , 332 ), ( 853 , 575 ), ( 408 , 460 ), ( 910 , 609 ), ( 786 , 283 ), ( 782 , 863 ), ( 552 , 341 ), ( 792 , 976 ), ( 160 , 394 ), ( 779 , 339 ), ( 363 , 494 ), ( 164 , 465 ), ( 361 , 862 ), ( 93 , 357 ), ( 913 , 359 ), ( 2 , 395 ), ( 466 , 107 ), ( 513 , 426 ), ( 36 , 188 ), ( 378 , 419 ), ( 977 , 105 ), ( 663 , 648 ), ( 540 , 53 ), ( 931 , 656 ), ( 24 , 641 ), ( 132 , 296 ), ( 175 , 754 ), ( 302 , 217 ), ( 177 , 60 ), ( 519 , 857 ), ( 995 , 335 ), ( 541 , 70 ), ( 852 , 66 ), ( 458 , 879 ), ( 471 , 349 ), ( 9 , 765 ), ( 796 , 435 ), ( 807 , 130 ), ( 901 , 501 ), ( 322 , 582 ), ( 520 , 763 ), ( 875 , 869 ), ( 938 , 708 ), ( 863 , 366 ), ( 909 , 434 ), ( 977 , 678 ), ( 354 , 760 ), ( 9 , 296 ), ( 335 , 212 ), ( 428 , 769 ), ( 789 , 452 ), ( 672 , 697 ), ( 121 , 972 ), ( 193 , 231 ), ( 825 , 985 ), ( 56 , 350 ), ( 635 , 725 ), ( 544 , 871 ), ( 316 , 716 ), ( 323 , 344 ), ( 741 , 860 ), ( 826 , 585 ), ( 460 , 36 ), ( 395 , 711 ), ( 361 , 969 ), ( 796 , 598 ), ( 199 , 913 ), ( 557 , 213 ), ( 362 , 307 ), ( 471 , 423 ), ( 660 , 902 ), ( 108 , 682 ), ( 162 , 35 ), ( 734 , 210 ), ( 2 , 952 ), ( 108 , 127 ), ( 867 , 752 ), ( 788 , 49 ), ( 359 , 500 ), ( 128 , 677 ), ( 57 , 836 ), ( 293 , 898 ), ( 747 , 64 ), ( 419 , 760 ), ( 611 , 219 ), ( 388 , 410 ), ( 750 , 987 ), ( 442 , 706 ), ( 335 , 945 ), ( 378 , 503 ), ( 727 , 899 ), ( 857 , 991 ), ( 469 , 863 ), ( 29 , 933 ), ( 961 , 602 ), ( 587 , 480 ), ( 835 , 344 ), ( 503 , 114 ), ( 667 , 789 ), ( 802 , 490 ), ( 803 , 149 ), ( 325 , 624 ), ( 231 , 704 ), ( 414 , 530 ), ( 664 , 219 ), ( 491 , 404 ), ( 795 , 610 ), ( 249 , 853 ), ( 407 , 950 ), ( 780 , 588 ), ( 506 , 118 ), ( 731 , 221 ), ( 284 , 864 ), ( 808 , 162 ), ( 893 , 724 ), ( 275 , 425 ), ( 466 , 403 ), ( 771 , 596 ), ( 632 , 556 ), ( 374 , 946 ), ( 390 , 156 ), ( 323 , 294 ), ( 811 , 662 ), ( 426 , 763 ), ( 637 , 176 ), ( 994 , 590 ), ( 963 , 335 ), ( 225 , 222 ), ( 65 , 765 ), ( 975 , 696 ), ( 519 , 533 ), ( 733 , 974 ), ( 653 , 354 ), ( 664 , 533 ), ( 309 , 239 ), ( 180 , 214 ), ( 187 , 967 ), ( 846 , 125 ), ( 831 , 691 ), ( 647 , 133 ), ( 135 , 10 ), ( 944 , 650 ), ( 773 , 102 ), ( 430 , 144 ), ( 781 , 941 ), ( 705 , 62 ), ( 162 , 9 ), ( 309 , 548 ), ( 869 , 818 ), ( 60 , 651 ), ( 290 , 642 ), ( 241 , 235 ), ( 495 , 877 ), ( 350 , 229 ), ( 206 , 221 ), ( 951 , 416 ), ( 803 , 303 ), ( 504 , 475 ), ( 577 , 471 ), ( 643 , 902 ), ( 351 , 369 ), ( 23 , 36 ), ( 303 , 174 ), ( 572 , 349 ), ( 898 , 645 ), ( 734 , 443 ), ( 764 , 929 ), ( 159 , 849 ), ( 772 , 831 ), ( 468 , 25 ), ( 771 , 535 ), ( 714 , 751 ), ( 784 , 622 ), ( 673 , 716 ), ( 395 , 598 ), ( 962 , 504 ), ( 306 , 942 ), ( 603 , 372 ), ( 400 , 342 ), ( 351 , 425 ), ( 614 , 475 ), ( 441 , 483 ), ( 702 , 7 ), ( 398 , 87 ), ( 170 , 57 ), ( 714 , 538 ), ( 193 , 275 ), ( 601 , 365 ), ( 441 , 246 ), ( 330 , 966 ), ( 64 , 950 ), ( 442 , 920 ), ( 784 , 432 ), ( 196 , 768 ), ( 437 , 911 ), ( 142 , 281 ), ( 415 , 306 ), ( 881 , 350 ), ( 563 , 266 ), ( 247 , 160 ), ( 411 , 816 ), ( 751 , 772 ), ( 741 , 347 ), ( 83 , 69 ), ( 234 , 632 ), ( 337 , 531 ), ( 662 , 476 ), ( 126 , 662 ), ( 926 , 826 ), ( 481 , 946 ), ( 809 , 674 ), ( 86 , 151 ), ( 912 , 633 ), ( 754 , 339 ), ( 674 , 7 ), ( 251 , 860 ), ( 991 , 166 ), ( 532 , 556 ), ( 113 , 490 ), ( 564 , 719 ), ( 73 , 411 ), ( 591 , 217 ), ( 501 , 335 ), ( 212 , 167 ), ( 314 , 874 ), ( 141 , 106 ), ( 489 , 451 ), ( 810 , 145 ), ( 990 , 330 ), ( 555 , 126 ), ( 349 , 987 ), ( 127 , 426 ), ( 311 , 328 ), ( 551 , 469 ), ( 603 , 125 ), ( 173 , 95 ), ( 303 , 275 ), ( 968 , 931 ), ( 822 , 637 ), ( 541 , 732 ), ( 140 , 171 ), ( 303 , 838 ), ( 788 , 520 ), ( 239 , 692 ), ( 238 , 370 ), ( 619 , 805 ), ( 280 , 397 ), ( 590 , 301 ), ( 784 , 950 ), ( 824 , 604 ), ( 498 , 567 ), ( 39 , 463 ), ( 302 , 298 ), ( 276 , 984 ), ( 738 , 383 ), ( 209 , 282 ), ( 799 , 532 ), ( 975 , 865 ), ( 456 , 616 ), ( 455 , 753 ), ( 93 , 684 ), ( 110 , 309 ), ( 841 , 259 ), ( 995 , 907 ), ( 578 , 578 ), ( 868 , 501 ), ( 240 , 381 ), ( 525 , 930 ), ( 435 , 197 ), ( 678 , 382 ), ( 57 , 490 ), ( 136 , 152 ), ( 375 , 363 ), ( 63 , 652 ), ( 728 , 709 )]

    positions = generate_position(500, 300, 300)

    print(positions)

    k = 200
    c = 1

    search(positions, k , c)



def generate_position(count, x_max, y_max):
    positions = []
    for x in xrange(count):
        positions.append((randint(0, x_max), randint(0, y_max)))
    return positions


def search(positions, k, c):
    #solution = state_search(positions, k, c)
    solution = local_search(positions, k, c)

    draw_plot(positions, solution)
    print(solution)


def run(mode):
    func = enumToFunc[mode]
    func()

# Exemple de comment utiliser le
#@do_profile(follow=[startSearch])
def run_line_profiler():
    startSearch()

def run_cprofiler():
    cProfile.run('startSearch()', 'result')
    p = pstats.Stats('result')
    p.sort_stats('time').print_stats(20)


class RunType():
    c_profile = 1
    line_profile = 2
    normal = 3

enumToFunc = {RunType.c_profile: run_cprofiler, RunType.line_profile: run_line_profiler, RunType.normal: startSearch}

run(RunType.normal)
#startSearch()


