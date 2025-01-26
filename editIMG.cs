using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;



//콤보박스를 넣어서 칼라를 모노로 바꾸는 필터추가


namespace PixelPainter
{   
    
    enum ImageOperation
    {
        OpAdd = 0,
        OpSubtract,
        OpMultiply,
        OpDivide,
        OpMax,
        OpMin,
        OpAbs,
        OpAbDiff,
        and,
        or,
        xor,
        not,
        compare,
    }

    enum ImageFilter
    {
        FilterBlur = 0,
        FilterBoxFilter,
        FilterMedianBlur,
        FilterGaussianBlur,
        FilterBilateral,
        FilterSobel,
        FilterScharr,
        FilterLaplacian,
        FilterCanny,

    }


    public partial class editIMG : Form
    {        
        public editIMG()
        {
            InitializeComponent();
        }

        private bool isImageLoaded = false; //불러온이미지없이 연산시 예외처리
        private bool isImageOp = false;     //저장할 연산이미지없을시 예외처리
        Mat blur = new Mat();
        Mat dst = new Mat();

        private void openBTN_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {  //DialogResult는 windows forms 라이브러리에서 제공하는 열거형.
                //대화상자(Dialog)에서 반환되는 값 표현
                pictureBox1.Load(openFileDialog1.FileName);

                Mat src1 = Cv2.ImRead(openFileDialog1.FileName);
                pictureBox1.Image = BitmapConverter.ToBitmap(src1);

                isImageLoaded = true;  

            }
        }

        private void saveBTN_Click(object sender, EventArgs e)
        {
            if (!isImageOp)
            {
                MessageBox.Show("저장할 이미지가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // SaveFileDialog 설정
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Title = "이미지 저장",
                Filter = "JPEG Files|*.jpg|PNG Files|*.png|Bitmap Files|*.bmp|All Files|*.*",
                DefaultExt = "jpg",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(filePath).ToLower(); // 확장자 추출

                // 저장할 Mat 객체 (예: dst에 연산된 결과가 있다고 가정)
                Mat imageToSave = dst;

                try
                {
                    // 확장자에 따른 저장 형식 설정
                    switch (fileExtension)
                    {
                        case ".jpg":
                        case ".jpeg":
                            Cv2.ImWrite(filePath, imageToSave, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90)); // JPEG 품질 설정
                            break;

                        case ".png":
                            Cv2.ImWrite(filePath, imageToSave, new ImageEncodingParam(ImwriteFlags.PngCompression, 3)); // PNG 압축 설정
                            break;

                        case ".bmp":
                            Cv2.ImWrite(filePath, imageToSave);
                            break;

                        default:
                            MessageBox.Show("지원하지 않는 파일 형식입니다. JPG, PNG, BMP만 저장 가능합니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                    }

                    MessageBox.Show("이미지가 성공적으로 저장되었습니다.", "성공", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"이미지를 저장하는 동안 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpcomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isImageLoaded)
            {
                MessageBox.Show("먼저 이미지를 열어주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Mat src1 = Cv2.ImRead(openFileDialog1.FileName);
            Mat src2 = new Mat(src1.Size(), MatType.CV_8UC3, new Scalar(0, 0, 30));


            ImageOperation selType = (ImageOperation)OpcomboBox.SelectedIndex;

            switch (selType)
            {
                case ImageOperation.OpAdd:
                    Cv2.Add(src1, src2, dst);
                    break;
                case ImageOperation.OpSubtract:
                    Cv2.Subtract(src1, src2, dst);
                    break;
                case ImageOperation.OpMultiply:
                    Cv2.Multiply(src1, src2, dst);
                    break;
                case ImageOperation.OpDivide:
                    Cv2.Divide(src1, src2, dst);
                    break;
                case ImageOperation.OpMax:
                    Cv2.Max(src1, src2, dst);
                    break;
                case ImageOperation.OpMin:
                    Cv2.Min(src1, src2, dst);
                    break;
                case ImageOperation.OpAbs:
                    Cv2.Multiply(src1, src2, dst);
                    Cv2.Abs(dst);
                    break;
                case ImageOperation.OpAbDiff:
                    Mat matMul = new Mat();
                    Cv2.Multiply(src1, src2, matMul);
                    Cv2.Absdiff(src1, matMul, dst);
                    break;
                case ImageOperation.and:
                    Cv2.BitwiseAnd(src1, src2, dst);
                    break;
                case ImageOperation.or:
                    Cv2.BitwiseOr(src1, src2, dst);
                    break;
                case ImageOperation.xor:
                    Cv2.BitwiseXor(src1, src2, dst);
                    break;
                case ImageOperation.not:
                    Cv2.BitwiseNot(src1, dst);
                    break;
                case ImageOperation.compare:
                    Cv2.Compare(src1, src2, dst, CmpType.EQ);
                    break;
            }

            // OpenCvSharp의 Mat을 Bitmap으로 변환하여 PictureBox에 출력
            pictureBox2.Image = BitmapConverter.ToBitmap(dst);
            isImageOp = true;

            // 연산 결과를 TextBox에 출력 (예: Mat의 픽셀 평균값)
            Scalar mean = Cv2.Mean(dst); // Mat의 평균값 계산
            textBox1.Text = $"Mean: {mean.Val0:F2}, {mean.Val1:F2}, {mean.Val2:F2}";

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isImageLoaded)
            {
                MessageBox.Show("먼저 이미지를 열어주세요.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Mat src1 = Cv2.ImRead(openFileDialog1.FileName);
            //Mat src = Cv2.ImRead("sparkler.png", ImreadModes.ReducedColor2);
            Mat src2 = new Mat(src1.Size(), MatType.CV_8UC3, new Scalar(0, 0, 30));

            Cv2.GaussianBlur(src1, blur, new OpenCvSharp.Size(3, 3), 1, 0, BorderTypes.Default);

            ImageFilter selType = (ImageFilter)FiltercomboBox.SelectedIndex;

            switch (selType)
            {
                case ImageFilter.FilterBlur:
                    Cv2.Blur(src1, dst, new OpenCvSharp.Size(9, 9), new OpenCvSharp.Point(-1, -1), BorderTypes.Default);
                    break;
                case ImageFilter.FilterBoxFilter:
                    Cv2.BoxFilter(src1, dst, MatType.CV_8UC3, new OpenCvSharp.Size(9, 9), new OpenCvSharp.Point(-1, -1), true, BorderTypes.Default);
                    break;
                case ImageFilter.FilterMedianBlur:
                    Cv2.MedianBlur(src1, dst, 9);
                    break;
                case ImageFilter.FilterGaussianBlur:
                    Cv2.GaussianBlur(src1, dst, new OpenCvSharp.Size(9, 9), 1, 1, BorderTypes.Default);
                    break;
                case ImageFilter.FilterBilateral:
                    Cv2.BilateralFilter(src1, dst, 9, 3, 3, BorderTypes.Default);
                    break;
                case ImageFilter.FilterSobel:
                    Cv2.Sobel(blur, dst, MatType.CV_32F, 1, 0, ksize: 3, scale: 1, delta: 0, BorderTypes.Default);
                    dst.ConvertTo(dst, MatType.CV_8UC1);
                    break;
                case ImageFilter.FilterScharr:
                    Cv2.Scharr(blur, dst, MatType.CV_32F, 1, 0, scale: 1, delta: 0, BorderTypes.Default);
                    dst.ConvertTo(dst, MatType.CV_8UC1);
                    break;
                case ImageFilter.FilterLaplacian:
                    Cv2.Laplacian(blur, dst, MatType.CV_32F, ksize: 3, scale: 1, delta: 0, BorderTypes.Default);
                    dst.ConvertTo(dst, MatType.CV_8UC1);
                    break;
                case ImageFilter.FilterCanny:
                    Cv2.Canny(blur, dst, 100, 200, 3, true);
                    break;
            }

            // OpenCvSharp의 Mat을 Bitmap으로 변환하여 PictureBox에 출력
            pictureBox2.Image = BitmapConverter.ToBitmap(dst);
            isImageOp = true;

            // 연산 결과를 TextBox에 출력 (예: Mat의 픽셀 평균값)
            Scalar mean = Cv2.Mean(dst); // Mat의 평균값 계산
            textBox1.Text = $"Mean: {mean.Val0:F2}, {mean.Val1:F2}, {mean.Val2:F2}";
        }
    }
}
